# 今天也是个好天气去种地，但是这个火箭筒是干什么的？

> 一款 2D 像素风"模拟经营 + 塔防战斗"游戏。

白天种植作物赚取金币，夜晚抵御怪物进攻围栏，撑过 7 天通关。

![游戏截图](docs/screenshot.png)

## 🎮 玩法

- **白天**：种植小麦和茄子，收获金币，去商店升级武器和设施
- **夜晚**：怪物从围栏外进攻，全部消灭才能进入下一天
- **目标**：撑过 7 天，守住你的农场

### 核心循环
种植 → 收获 → 金币 → 升级 → 战斗 → 撑过夜晚 → 下一天

## 🔫 武器系统

| 武器 | 特点 | 解锁价格 |
|---|---|---|
| 手枪 | 单发，中等伤害 | 默认 |
| 霰弹枪 | 一次发射 5 颗扇形子弹 | 200 金币 |
| 步枪 | 连发，高射速 | 500 金币 |
| 火箭筒 | 范围爆炸伤害 | 1000 金币 |

## 🛠️ 技术栈

- **引擎**：Unity 2022.3.62f3c1
- **语言**：C#
- **核心系统**：Tilemap、Particle System、Cinemachine、TextMeshPro

## 📁 项目结构
Assets/
├── Scripts/ # 所有 C# 脚本
├── Data/ # ScriptableObject 数据（武器、作物、敌人）
├── Prefabs/ # 预制体
├── Sprites/ # 像素美术
├── Audio/ # 音效和 BGM
└── Scenes/ # 场景

ProjectSettings/ # Unity 项目设置
Packages/ # 依赖包

## 🚀 如何运行

1. 用 **Unity Hub** 安装 **Unity 2022.3.62f3c1**
2. 克隆仓库：
   ```bash
   git clone https://github.com/Shanheyi/NiceWeatherRocketFarm.git
用 Unity 打开项目文件夹
打开 Assets/Scenes/SampleScene.unity
点 Play 运行


作者
郑洪博

GitHub：@Shanheyi
邮箱：1799325976@qq.com