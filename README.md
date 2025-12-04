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
### T_ObjectFunc
```bash
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[T_ObjectFunc]') AND type IN ('U'))
	DROP TABLE [dbo].[T_ObjectFunc]
GO

CREATE TABLE [dbo].[T_ObjectFunc] (
  [Id] bigint  NOT NULL,
  [ObjectId] bigint  NOT NULL,
  [Code] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [Name] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [Method] varchar(100) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [CreateId] bigint  NOT NULL,
  [CreateName] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [CreateTime] datetime  NOT NULL,
  [ModifierId] bigint  NULL,
  [ModifilerName] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [ModifilcationTime] datetime  NULL
)
GO

ALTER TABLE [dbo].[T_ObjectFunc] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'业务对象Id',
'SCHEMA', N'dbo',
'TABLE', N'T_ObjectFunc',
'COLUMN', N'ObjectId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'编码',
'SCHEMA', N'dbo',
'TABLE', N'T_ObjectFunc',
'COLUMN', N'Code'
GO

EXEC sp_addextendedproperty
'MS_Description', N'名称',
'SCHEMA', N'dbo',
'TABLE', N'T_ObjectFunc',
'COLUMN', N'Name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'方法表示',
'SCHEMA', N'dbo',
'TABLE', N'T_ObjectFunc',
'COLUMN', N'Method'
GO


-- ----------------------------
-- Primary Key structure for table T_ObjectFunc
-- ----------------------------
ALTER TABLE [dbo].[T_ObjectFunc] ADD CONSTRAINT [PK__T_Object__3214EC072516A23D] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO
```
### T_Role
```bash
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[T_Role]') AND type IN ('U'))
	DROP TABLE [dbo].[T_Role]
GO

CREATE TABLE [dbo].[T_Role] (
  [Id] bigint  NOT NULL,
  [Code] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [Name] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [Property] tinyint  NOT NULL,
  [RoleType] tinyint  NOT NULL,
  [CreateOrg] bigint  NOT NULL,
  [CreateId] bigint  NOT NULL,
  [CreateName] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [CreateTime] datetime  NOT NULL,
  [ModifierId] bigint  NULL,
  [ModifilerName] varchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [ModifilcationTime] datetime  NULL
)
GO

ALTER TABLE [dbo].[T_Role] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色Id',
'SCHEMA', N'dbo',
'TABLE', N'T_Role',
'COLUMN', N'Id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色编码',
'SCHEMA', N'dbo',
'TABLE', N'T_Role',
'COLUMN', N'Code'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色名称',
'SCHEMA', N'dbo',
'TABLE', N'T_Role',
'COLUMN', N'Name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色属性 1 公共 0 私有',
'SCHEMA', N'dbo',
'TABLE', N'T_Role',
'COLUMN', N'Property'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色类型 0 普通角色 1 管理员',
'SCHEMA', N'dbo',
'TABLE', N'T_Role',
'COLUMN', N'RoleType'
GO


-- ----------------------------
-- Primary Key structure for table T_Role
-- ----------------------------
ALTER TABLE [dbo].[T_Role] ADD CONSTRAINT [PK__T_Role__3214EC07D14A9F74] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO
```
### T_RoleObject
```bash
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[T_RoleObject]') AND type IN ('U'))
	DROP TABLE [dbo].[T_RoleObject]
GO

CREATE TABLE [dbo].[T_RoleObject] (
  [Id] bigint  NOT NULL,
  [RoleId] bigint  NOT NULL,
  [ObjectId] bigint  NOT NULL
)
GO

ALTER TABLE [dbo].[T_RoleObject] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Primary Key structure for table T_RoleObject
-- ----------------------------
ALTER TABLE [dbo].[T_RoleObject] ADD CONSTRAINT [PK__T_RoleOb__3214EC07C6ECAF5B] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO
```
### T_User
```bash
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[T_User]') AND type IN ('U'))
	DROP TABLE [dbo].[T_User]
GO

CREATE TABLE [dbo].[T_User] (
  [Id] bigint  NOT NULL,
  [Code] varchar(20) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [Name] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [Sex] tinyint  NOT NULL,
  [Mobile] varchar(20) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [Status] tinyint  NOT NULL,
  [Type] tinyint  NOT NULL,
  [LinkId] bigint  NOT NULL,
  [Password] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [PasswordHash] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [StaffId] bigint  NULL,
  [CreateOrg] bigint  NOT NULL,
  [CreateId] bigint  NOT NULL,
  [CreateName] varchar(20) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [CreateTime] datetime  NOT NULL,
  [ModifierId] bigint  NULL,
  [ModifilerName] varchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [ModifilcationTime] datetime DEFAULT '' NULL
)
GO

ALTER TABLE [dbo].[T_User] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户工号',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'Code'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户名称',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'Name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'性别 0 女 1男',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'Sex'
GO

EXEC sp_addextendedproperty
'MS_Description', N'手机号',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'Mobile'
GO

EXEC sp_addextendedproperty
'MS_Description', N'状态 1 启用 0 禁用',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'Status'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户类型 0 集团 1 供应商',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'Type'
GO

EXEC sp_addextendedproperty
'MS_Description', N'关联Id 供应商填供应商Id 不是供应商填角色Id',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'LinkId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'密码密文',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'Password'
GO

EXEC sp_addextendedproperty
'MS_Description', N'密码hash',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'PasswordHash'
GO

EXEC sp_addextendedproperty
'MS_Description', N'员工Id',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'StaffId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建组织',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'CreateOrg'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建人Id',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'CreateId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建人名称',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'CreateName'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'CreateTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改人Id',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'ModifierId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改人名称',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'ModifilerName'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改时间',
'SCHEMA', N'dbo',
'TABLE', N'T_User',
'COLUMN', N'ModifilcationTime'
GO


-- ----------------------------
-- Primary Key structure for table T_User
-- ----------------------------
ALTER TABLE [dbo].[T_User] ADD CONSTRAINT [PK__T_User__3214EC0786C66C5F] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO
```
### T_UserRole
```bash
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[T_UserRole]') AND type IN ('U'))
	DROP TABLE [dbo].[T_UserRole]
GO

CREATE TABLE [dbo].[T_UserRole] (
  [Id] bigint  NOT NULL,
  [UserId] bigint  NOT NULL,
  [RoleId] bigint  NOT NULL
)
GO

ALTER TABLE [dbo].[T_UserRole] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Primary Key structure for table T_UserRole
-- ----------------------------
ALTER TABLE [dbo].[T_UserRole] ADD CONSTRAINT [PK__T_UserRo__3214EC07DCDCA16A] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO
```

