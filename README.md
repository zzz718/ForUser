# ForUser
## SqlServer数据库建表sql
### T_Object
```bash
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[T_Object]') AND type IN ('U'))
	DROP TABLE [dbo].[T_Object]
GO
CREATE TABLE [dbo].[T_Object] (
  [Id] bigint  NOT NULL,
  [Code] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [Name] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [CreateName] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [CreateTime] datetime  NOT NULL,
  [ModifierId] bigint  NULL,
  [ModifilerName] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [ModifilcationTime] datetime  NULL,
  [CreateId] bigint  NOT NULL
)
GO
ALTER TABLE [dbo].[T_Object] SET (LOCK_ESCALATION = TABLE)
GO
EXEC sp_addextendedproperty
'MS_Description', N'对象编码',
'SCHEMA', N'dbo',
'TABLE', N'T_Object',
'COLUMN', N'Code'
GO
EXEC sp_addextendedproperty
'MS_Description', N'对象名称',
'SCHEMA', N'dbo',
'TABLE', N'T_Object',
'COLUMN', N'Name'
GO
-- ----------------------------
-- Primary Key structure for table T_Object
-- ----------------------------
ALTER TABLE [dbo].[T_Object] ADD CONSTRAINT [PK__T_Object__3214EC07A46D92C3] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO
```

