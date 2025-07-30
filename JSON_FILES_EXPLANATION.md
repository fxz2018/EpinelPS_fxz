# JSON文件数量差异详解

## 🤔 **为什么是7336个文件而不是57个？**

### 📊 **文件构成分析**

| 文件类型 | 数量 | 占比 | 说明 |
|---------|------|------|------|
| **对话文件** | 6,386 | 87% | 角色咨询对话、互动文本 |
| **剧情文件** | 3,698 | 50% | 主线、支线、事件剧情 |
| **战斗数据** | 138 | 2% | 关卡波次、敌人配置 |
| **核心数据表** | 60+ | 1% | 角色、装备、技能等核心数据 |

*注：对话和剧情文件有重叠，总计约7336个文件*

## 🎯 **57个 vs 7336个的区别**

### **57个文件** - 代码中加载的核心表
```csharp
[LoadRecord("CharacterTable.json", "id")]
[LoadRecord("ItemEquipTable.json", "id")]  
[LoadRecord("UserExpTable.json", "level")]
// ... 共约57个核心数据表
```

这些是游戏逻辑**必需**的核心数据表，包含：
- 角色基础数据
- 装备属性
- 技能信息
- 关卡配置
- 奖励设置

### **7336个文件** - 完整游戏数据包
StaticData.pack包含**完整的游戏内容**，包括：

#### 1. **对话文件** (6,386个)
```
AttractiveCounselDialogTable.d_counseling_alice_01.json
AttractiveCounselDialogTable.d_counseling_alice_02.json
...
```
- **作用**: 角色咨询对话、好感度对话
- **特点**: 每个角色每个对话场景一个文件
- **数量**: 约100个角色 × 20个对话场景 × 多个版本

#### 2. **剧情文件** (3,698个)
```
MainStoryScenarioDialogTable.d_main_01_01.json
SideStoryScenarioDialogTable.d_side_01_01.json
EventScenarioDialogTable.1ann_mini_prologue.json
```
- **作用**: 主线剧情、支线剧情、活动剧情
- **特点**: 按章节、场景细分
- **包含**: 对话文本、角色表情、背景音乐等

#### 3. **战斗数据** (138个)
```
WaveDataTable.wave_campaign_normal_001.json
WaveDataTable.wave_tower_elysion_001.json
```
- **作用**: 关卡敌人配置、战斗波次
- **分类**: 主线、困难、塔防、活动等

## 🔧 **优化方案**

### **方案1: 只提取核心数据表**

修改 `GameData.cs` 中的配置：
```csharp
private bool ExtractMainTablesOnly = true; // 改为true
```

**效果**：
- ✅ 只提取约60个核心数据表
- ✅ 文件数量大幅减少
- ✅ 便于查看和分析
- ❌ 无法查看对话和剧情内容

### **方案2: 分类提取**

可以进一步修改代码，按类型分别提取：
```csharp
// 只提取对话文件
if (entry.Name.Contains("Dialog"))
    
// 只提取剧情文件  
if (entry.Name.Contains("Scenario"))

// 只提取核心数据表
if (mainTables.Contains(Path.GetFileName(entry.Name)))
```

## 📁 **文件命名规则解析**

### **`.d_` 前缀文件**
```
AttractiveCounselDialogTable.d_counseling_alice_01.json
```
- **d_**: Dialog的缩写，表示对话文件
- **counseling**: 咨询类型
- **alice**: 角色名称
- **01**: 对话序号

### **分表文件**
```
ArchiveProgressEventTable.1080300.json
```
- **1080300**: 事件ID或分类ID
- **作用**: 将大表按ID范围分割

### **本地化文件**
```
LocaleTable.d_locale_character_name.json
```
- **locale**: 本地化标识
- **character_name**: 角色名称本地化

## 🎮 **游戏中的实际用途**

### **核心数据表** - 游戏逻辑
- 角色属性计算
- 装备效果应用
- 技能伤害计算
- 关卡难度设置

### **对话文件** - 内容展示
- 角色咨询系统
- 好感度对话
- 互动剧情

### **剧情文件** - 故事内容
- 主线剧情播放
- 支线任务对话
- 活动剧情展示

## 💡 **建议**

### **开发调试** - 使用核心数据表
```csharp
private bool ExtractMainTablesOnly = true;
```
- 快速查看游戏数据结构
- 分析角色和装备属性
- 理解游戏机制

### **内容分析** - 使用完整数据包
```csharp
private bool ExtractMainTablesOnly = false;
```
- 研究游戏剧情内容
- 分析角色对话
- 了解完整游戏世界观

## 🔄 **切换方法**

1. **编辑配置**：修改 `GameData.cs` 中的 `ExtractMainTablesOnly` 值
2. **清理缓存**：删除 `cache` 目录
3. **重启服务器**：重新运行程序，会按新配置提取文件

这样您就可以根据需要选择提取模式，既能快速查看核心数据，也能深入研究完整内容！
