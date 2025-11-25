using Pgvector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK.Dtos
{
    /// <summary>
    /// 文档基本信息
    /// </summary>
    public class DocumentContent
    {
        public string FileName { get; set; }
        public List<ContentBlock> ContentBlocks { get; set; }
        public List<ImageData> Images { get; set; }
    }
    /// <summary>
    /// 文档内容块
    /// </summary>
    public class ContentBlock
    {
        public int Id { get; set; }
        public string ContentType { get; set; } // "text" or "image"
        public string ContentInfo { get; set; }
        public int PositionInfo { get; set; }
        public List<int> AssociatedImageIds { get; set; } = new List<int>();
        public int? ImageId { get; set; } // 如果是图片块
        public string BlockType { get; set; } // "sentence", "paragraph", "image", "table_cell"
        public int SentenceCount { get; set; }
        public Vector? Embedding { get; set; }
    }
    /// <summary>
    /// 图片数据
    /// </summary>
    public class ImageData
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }

    public class SearchResult
    {
        public string Query { get; set; }
        public List<MatchResult> Matches { get; set; }
    }

    public class MatchResult
    {
        public int ContentBlockId { get; set; }
        public string Content { get; set; }
        public int Position { get; set; }
        public string BlockType { get; set; }
        public double Similarity { get; set; }
        public string DocumentName { get; set; }
        public List<ImageInfo> AssociatedImages { get; set; }
    }

    public class ImageInfo
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Description { get; set; }
        public byte[] ImageData { get; set; } // 可选，根据需要返回
    }
}
