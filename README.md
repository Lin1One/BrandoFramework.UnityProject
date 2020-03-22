# BrandoFramework.UnityProject
**BrandoFramework** 是一套在Unity上进行游戏开发的框架代码。   
框架代码基于本人在日常学习的总结和归纳，持续更新和完善。  
使用Unity版本为 2018.3  

## 1. 框架目录结构  
### 1.1 BrandoFramework 文件夹
**FrameworkSrc** 存放框架代码  
**FrameworkRes** 存放框架所需资源，  如编辑器扩展所需的图标。  
**TechnologyStudyStudio** 为个人学习过程的一些记录，如 .net 的源码阅读笔记，Shader 编写，以及插件如 UGUI，MeshBaker，StrangeIoC 源码的阅读笔记。 

### 1.2 DatiFile 文件夹
**Dati** 在意大利语里是数据的意思，Dati 类型继承自Unity的 ScriptableObject 类型  
用于以可视化的形式编辑和保存各功能模块的配置数据，并在框架的模块设置窗口中展示  
**Editor** 文件夹保存仅在编辑器环境中读取的Dati文件，不会参与打包。  
**Play** 文件夹中的Dati文件所对应的Json会在打包时复制到SteamingAssets目录中，并在运行时读取。  

### 1.3 GameProjects 文件夹
**AssetDatabase** 文件夹存放游戏直接读取的资源。如预制体，Shader  
**OriginAssets** 文件夹存放预制体所引用的材质，贴图，模型资源，以及用于生成UI界面的PSD资源  
**Src** 文件夹存放游戏相关代码
游戏项目相关资源。

## 2. 框架代码结构
客户端框架代码位于 FrameworkSrc/Client 文件夹中，框架分为 Core 部分和 Module 部分  
在 Core 和每个 Module 中都各分为 Editor，Runtime 两部分代码。  
另外，部分通用代码放在 FrameworkSrc/Common 中。  
在开发过程中，本框架采取 **核心代码 Core + 可选 Module** 的形式，将代码导入工程内。

### 2.1 框架核心代码 Core

实现功能 | 描述
:-: | :-: 
Injector 注入器  |  使存在依赖关系的模块解耦，通过接口传递实现控制反转，并且用于获取各功能模块的接口。
资源管理模块  |  提供各类资源的加载，卸载API;可配置资源加载策略
AssetBundle 模块  |提供AssetBundle加载、卸载API，Editor环境中可根据配置文件，对游戏项目的各目录进行多种策略的打包。
事件模块  |  线程安全，事件队列读取数量可控，提供泛型接口避免值类型传参产生装拆箱
计时器  |  单次，计数，不限次数计时器，线程安全
游戏启动器  |  游戏入口基类，在开始游戏时，基础底层模块接口-类型映射，并自动读取相应模块的配置数据。
Dati 功能代码  |  Dati 基类，并可以 Json 形式保存，避免代码重构导致ScriptableObject数据丢失
基础接口的定义  |  IReset，IModule 等基础接口的定义
编辑器扩展窗口模板  |  Editor环境代码，自定义编辑器窗口的基类，如带侧边栏Menu的编辑器窗口
代码生成器  |  自动构建含有代码头，包含父类，接口等一系列代码文件。

### 2.2 功能模块代码 Module

实现功能 | 描述
 :-:| :-: 
数据表模块 Runtime  | 提供表格数据的获取API
数据表模块 Editor  | 根据配置文件的读表规则，读取数据表格，自动生成对应的代码，及二进制数据。
UI模块 Runtime  | 提供开关UI界面，获取UI界面数据模型，界面逻辑控制类型实例的 API，可同步加载或分帧分块显示加载UI。
UI模块 Editor  | 根据 PSD 自动生成UI界面，并保存至预制体，或自定义的结构（用于分帧加载），生成UI开发所需的界面，数据模型，逻辑类型相关的代码
网络模块 Runtime  | 提供连接，关闭，发送，监听网络消息的 API，可自定义添加上下行数据加工处理器，适应不同项目的不同需求。
网络模块 Editor  | 提供一个可视化的网络协议预览界面，并根据Pb协议，自动生成单个/批量网络协议代码。
场景管理模块 Runtime  | 提供加载，卸载，预加载场景等 API，可通过对地图分块的自定义Scene数据结构，实现快速加载可视范围内的场景物体，其加载范围可配置
场景管理模块 Editor  |  可对 Scene 自动分割生成对应的地图分块文件，保存物体位置，光照贴图信息；还可对场景内物体批量合并。
行为树模块  | 游戏角色行为控制行为树。

### 2.3 通用代码 Common
此处“通用”相对于客户端而言，FrameworkSrc 中，分为 Client，Common，Server，即 Common 在客户端框架及本地 C# 服务器代码中均可用。  
此处为各种工具类脚本如IO，反射，Json，Md5，序列化等 Utility 代码，还有泛型对象池，.net 原生类型的扩展方法。  


## 3. TechnologyStudyStudio 
此文件夹为个人学习过程中的一些记录，如 .net 源码阅读笔记，Unity 内置的 cginc 阅读笔记，练手写的各种 shader，以及 LeetCode 刷题笔记。
该部分与框架无关。（其实:)就是懒得开多一个项目hhh)
