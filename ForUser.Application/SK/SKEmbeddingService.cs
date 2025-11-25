using ForUser.Domains.Kernels;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.Extensions.AI;
using Pgvector;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ForUser.Application.SK.Dtos;
using ForUser.Util;
using Microsoft.Extensions.Configuration;
using ForUser.Domains.Commons;
using ForUser.Domains.Kernels.Entities;

namespace ForUser.Application.SK
{
    public class SKEmbeddingService : ISKEmbeddingService
    {
        private readonly KernelFactory _kernelFactory;

        private readonly SnowIdGenerator _snowIdGenerator;
        private  ITextEmbeddingGenerationService embeddingService;

        private readonly TextProcessingOptions _textOptions;
        private Kernel _kernel;
        private readonly IKnowLedgeRepository _knowLedgeRepository;

        public SKEmbeddingService(KernelFactory kernelFactory, IConfiguration configuration, SnowIdGenerator snowIdGenerator, IKnowLedgeRepository knowLedgeRepository) // 注入 Singleton Kernel
        {
            _kernelFactory = kernelFactory;
            _kernel = _kernelFactory.GetKernelForModel("embedding");
            
            _snowIdGenerator = snowIdGenerator;
            _knowLedgeRepository = knowLedgeRepository;

            _textOptions = configuration.GetSection("TextProcessing").Get<TextProcessingOptions>()
                   ?? new TextProcessingOptions();
            embeddingService = _kernel.GetRequiredService<ITextEmbeddingGenerationService>();
        }

        public async Task MessageEmbeddingAsync(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("文件为空");
            }

            try
            {
                // 1. 获取文件名
                string fileName = Path.GetFileName(file.FileName);

                // 2. 获取文件类型（MIME类型）
                string contentType = file.ContentType;

                // 3. 获取文件内容
                using var stream = file.OpenReadStream();


                var documentContent =  await ProcessDocxFileAsync(stream, fileName);

                var doc_Id = _snowIdGenerator.NextId();

                var knowLedgeEntites = new List<EmbeddingEntity>();
                
                documentContent.ContentBlocks.ForEach(c => 
                {
                    knowLedgeEntites.Add(new EmbeddingEntity()
                    {
                        Doc_Name = fileName,
                        Doc_Id = doc_Id,
                        Doc_Content = c.ContentInfo,
                        Embedding = c.Embedding
                    });
                });

                foreach (var entity in knowLedgeEntites)
                {
                    await _knowLedgeRepository.AddAsync(entity);
                }


            }
            catch (Exception ex)
            {
                // 异常处理
                throw new ApplicationException($"处理文件时出错: {ex.Message}", ex);
            }

        }
        /// <summary>
        /// 处理docx文件
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<DocumentContent> ProcessDocxFileAsync(Stream fileStream, string fileName)
        {
            var documentContent = new DocumentContent
            {
                FileName = fileName,
                ContentBlocks = new List<ContentBlock>(),
                Images = new List<ImageData>()
            };

            int currentPosition = 0;
            var currentImageIds = new List<int>();
            var allSentences = new List<string>();

            using (var wordDoc = WordprocessingDocument.Open(fileStream, false))
            {
                var mainPart = wordDoc.MainDocumentPart;

                // 先提取所有图片
                await ExtractAllImagesAsync(mainPart, documentContent);

                // 第一步：收集所有句子
                foreach (var element in mainPart.Document.Body.Elements())
                {
                    if (element is Paragraph paragraph)
                    {
                        var paragraphText = GetParagraphText(paragraph);
                        if (!string.IsNullOrWhiteSpace(paragraphText))
                        {
                            var sentences = SplitIntoSentences(paragraphText);
                            allSentences.AddRange(sentences.Where(s => !string.IsNullOrWhiteSpace(s) && s.Trim().Length > 3));
                        }
                    }
                }

                // 第二步：每4句切分一次
                for (int i = 0; i < allSentences.Count; i += 4)
                {
                    var batch = allSentences.Skip(i).Take(4).ToList();
                    if (!batch.Any()) continue;

                    var combinedContent = string.Join("", batch.Select(s =>
                        s.EndsWith( '。') || s.EndsWith('！')|| s.EndsWith('？') ? s : s + "。"));

                    // 生成向量
                    var textVector  = new Vector((await embeddingService.GenerateEmbeddingAsync(combinedContent)).ToArray());

                    // 创建文本块
                    documentContent.ContentBlocks.Add(new ContentBlock
                    {
                        Id = documentContent.ContentBlocks.Count,
                        ContentType = "text",
                        ContentInfo = combinedContent,
                        PositionInfo = currentPosition,
                        AssociatedImageIds =currentImageIds,
                        BlockType = "sentence_group",
                        SentenceCount = batch.Count,
                        Embedding = textVector
                    });

                    currentPosition++;
                }
            }

            return documentContent;
        }
        /// <summary>
        /// 提取docx文档中的所有图片并将图片内容和图片ID保存到文档内容中
        /// </summary>
        /// <param name="mainPart"></param>
        /// <param name="documentContent"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private async Task ExtractAllImagesAsync(MainDocumentPart mainPart, DocumentContent documentContent)
        {
            var imageIndex = 0;
            var imageDictionary = new Dictionary<string, int>();
            // 遍历所有图片部件
            foreach (var imagePart in mainPart.ImageParts)
            {
                try
                {
                    using var imageStream = new MemoryStream();
                    await imagePart.GetStream().CopyToAsync(imageStream);
                    //回到开始位置
                    imageStream.Position = 0;
                    //每次递增总数作为Id
                    var imageId = documentContent.Images.Count;
                    var imageData = new ImageData
                    {
                        Id = imageId,
                        FileName = $"image_{imageId}_{Guid.NewGuid()}{ImageHelper.GetImageExtension(imagePart.ContentType)}",
                        ContentType = imagePart.ContentType,
                        Content = imageStream.ToArray()
                    };
                    //将图片添加到文档内容中
                    documentContent.Images.Add(imageData);
                    imageDictionary[imagePart.Uri.ToString()] = imageId;
                    //图片索引+1
                    imageIndex++;
                }
                catch(Exception ex)
                {
                    throw new ApplicationException($"处理图片时出错: {ex.Message}", ex);
                }
            }
        }
        private string GetParagraphText(Paragraph paragraph)
        {
            var textElements = paragraph.Descendants<Text>();
            return string.Join("", textElements.Select(t => t.Text));
        }
        /// <summary>
        /// 创建文本块
        /// </summary>
        /// <param name="documentContent"></param>
        /// <param name="sentences"></param>
        /// <param name="currentImageIds"></param>
        /// <param name="position"></param>
        /// <param name="blockType"></param>
        /// <returns></returns>
        private async Task CreateTextBlockFromSentencesAsync(
            DocumentContent documentContent,
            List<string> sentences,
            List<int> currentImageIds,
            int position,string blockType)
        {
            if (!sentences.Any() || sentences.Count > _textOptions.SentencesPerBlock)
                return;

            var combinedContent = string.Join("", sentences.Select(s =>
                    s.EndsWith("。") || s.EndsWith("!") || s.EndsWith("?") ? s : s + "。"));

            var textVector = await embeddingService.GenerateEmbeddingAsync(combinedContent);

            documentContent.ContentBlocks.Add(new ContentBlock
            {
                Id = documentContent.ContentBlocks.Count,
                ContentType = "text",
                ContentInfo = combinedContent,
                PositionInfo = position,
                AssociatedImageIds = currentImageIds,
                BlockType = blockType
            });
        }
        /// <summary>
        /// 按照配置文件切分文本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private List<string> SplitIntoSentences(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            var sentences = new List<string>();
            var currentSentence = new StringBuilder();

            foreach (char c in text)
            {
                currentSentence.Append(c);

                if (_textOptions.SentenceSeparators.Contains(c))
                {
                    var sentence = currentSentence.ToString().Trim();
                    if (sentence.Length >= _textOptions.MinSentenceLength)
                    {
                        sentences.Add(sentence);
                    }
                    currentSentence.Clear();
                }
            }

            if (currentSentence.Length >= _textOptions.MinSentenceLength)
            {
                sentences.Add(currentSentence.ToString().Trim());
            }

            return sentences;
        }

    }
}
