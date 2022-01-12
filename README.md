# Mini Game Test Field

## 说明

该项目为小游戏测试场地，用于确保小游戏的兼容性

说人话就是只要你的小游戏能在这里按测试标准正常跑，基本上就说明塞到主游戏里面就没什么问题了



## 开发信息

1. Unity版本：2019.4.32f1
2. 使用管线：**Universal Render Pipeline（URP）**
3. 插件依赖：
   * Universal RP（7.7.1）

4. 文件结构说明
   * Assets：Assets根节点
     * **TestField**：该文件夹下为主环境相关内容，小游戏实际测试时应不必考虑具体内容
       * Images：图片资源
       * Render：渲染管线资源
       * Scene：场景
       * Scripts：相关脚本
         * ENVMain.cs：主环境主控
         * EnvCollision.cs：主环境碰撞检测组件
         * MiniGameBase.cs：小游戏的基类，请确保小游戏继承子该基类并实现相应接口
         * MiniGameLoadMode.cs：枚举，指示小游戏的载入模式
         * GlobalSettings.cs：全局设置接口，控制游戏的通用设置
         * VNSimulation.cs：视觉小说内交互接口模拟器
       
     * **MiniGame**：小游戏相关文件夹
       * Scripts：示例小游戏相关文件夹
         * SampleGame.cs：示例小游戏的主代码
         * Wall.cs：示例小游戏的墙体组件
       * Images：图片资源
       * Prefabs：小游戏的预制体




## 使用说明

### 0. 可用类

#### 1. MiniGameBase

​		将该类作为小游戏的基类，并实现定义的接口

​		所有事件/方法均为public

​		后续接口可能有修改，但大致上应该不会有太大变化

```c#
/* 该事件表示小游戏已完成，在游戏完成时通过OnGameCompleted方法引发该事件，并附带状态码
   具体的后续操作由事件注册者决定*/
event Action<int> GameCompleted;


/* 该事件表示小游戏发起关闭请求，在游戏内关闭游戏小游戏通过OnCloseRequested方法引发该事件
   具体的关闭操作由于事件注册者决定*/
event Action CloseRequested;

/* 小游戏的主相机，如果小游戏内需要使用主相机，请访问此相机 */
Camera MainCamera; 
/* 小游戏的UI相机，如果小游戏内需要使用UI相机，请访问此相机（多用于Canvas） */
Camera UICamera; 

/* 载入小游戏的方法，在主环境请求加载小游戏时将调用此方法，并传递一个onCompleted回调与loadMode
   对于onCompleted，请确保在方法实现中调用此方法
   对于loadMode，可以根据其值调整小游戏的内容 */
abstract void LoadGame(Action onCompleted, MiniGameLoadMode loadMode);


/* 卸载小游戏的方法，在主环境请求卸载小游戏时将调用此方法，并传递一个onCompleted
   对于onCompleted，请确保在方法实现中调用此方法 */
abstract void UnloadGame(Action onCompleted);


/* 引发GameCompleted的辅助方法，传入一个int的参数用于指示小游戏完成时的返回状态码
   该方法为虚方法，可以根据需要覆写 */
virtual void OnGameCompleted(int status);


/* 引发CloseRequested的辅助方法，该方法为虚方法，可以根据需要覆写 */
virtual void OnCloseRequested();
```



#### 2. RankingManager

该类为静态帮助类，用于读写小游戏的排名信息

* 储存说明：

  储存时会将gameName转化为具体文件名fileName，将rankingRecords的复制到一个列表副本中，对列表副本进行排序后取前MaxRecords条记录，对其进行json序列化后写入到fileName中，json结构如下

```json
[
    {
        "Name": "阿草",
        "Score": 116
    },
    {
        "Name": "一个魂",
        "Score": 115
    },
    {
        "Name": "一个一个魂",
        "Score": 114
    }
]
```

* 读取说明：

  读取时会将gameName转化为具体文件名fileName，读取fileName并反序列化为List\<TRankingRecord\>，取前MaxRecords记录返回

```c#
/* 该字段指示存档列表的容量，当前为7，分别是五位姑娘、阿草与一个魂 */
int MaxRecords = 7;
    
/* 保存小游戏排行榜数据至指定文件 */
bool SaveRankInfo(List<RankingRecord> rankingRecords, string gameName);
    
/* 保存小游戏排行榜数据至指定文件方法的泛型版本，泛型TRankingRecord约束为继承自RankingRecord */
bool SaveRankInfo<TRankingRecord>(List<TRankingRecord> rankingRecords, string gameName);

/* 读取小游戏的排行榜数据，该方法为泛型方法，泛型TRankingRecord约束为继承自RankingRecord */
List<RankingRecord> LoadRankInfo(string gameName);

/* 读取小游戏的排行榜数据方法的泛型版本，泛型TRankingRecord约束为继承自RankingRecord */
List<TRankingRecord> LoadRankInfo<TRankingRecord>(string gameName);
```

* 相关类：

  **RankingRecord**

  该类为排名记录项的基类，若需要使用更复杂的排名记录来记录额外的信息，请继承自该类

  ```c#
  /* 玩家名 */
  string Name { get; set; }
  
  /* 得分 */
  int Score { get; set; }
  
  /* IComparable下的CompareTo方法，用于比较排名次序，该方法为虚方法，可以在派生类中重写 */
  int CompareTo(RankingRecord other);
  ```

  例如，如果你希望你的json文件中除了储存玩家名和得分外，还想储存本局游戏用时等信息，如下
  
  ```c#
  [
      {
          "Name": "阿草",
          "Score": 116,
          "Time": 5
      },
      {
          "Name": "一个魂",
          "Score": 115,
          "Time": 10
      },
      {
          "Name": "一个一个魂",
          "Score": 114,
          "Time": 15
      }
  ]
  ```
  
  你可以创建一个继承自**RankingRecord**的类，然后在读写排行榜信息时使用泛型版本的方法，如下
  
  ```c#
  public class MyRankingRecord : RankingRecord
  {
      public int Time {get; set;}
  }
  
  // 使用泛型方法保存
  RankingManager.SaveRankingRecords<MyRankingRecord>(ranking2, "MyGame");
  
  // 使用泛型方法读取
  var records = RankingManager.LoadRankingRecords<MyRankingRecord>("MyGame");
  ```
  



#### 3. AchievementManager

该类未静态类，用于管理游戏的成就系统

注意目前尚未确认成就的apiName，你可以使用任意自定义名称来测试，在正式接入后会调整apiName

```c#
// 查询指定成就是否解锁，返回一个bool值。在测试场景中，当成就不存在或成就为解锁时均为返回false
bool IsUnlocked(string apiName);

// 解锁成就，若成就已解锁无影响
void Unlock(string apiName);

// 锁定成就，若成就已锁定无影响
void Lock(string apiName);

// 获取成就进度，该项仅对进度型成就有效
int GetProgress(string apiName);

// 设置成就进度，该项仅对进度型成就有效
void SetProgress(string apiName, int value);
```

注：apiName为成就的API名称，该名称目前尚未定论



#### 4. **VNSimulation**

该类用于临时模拟小游戏与视觉小说的对话框调用等交互

正式合并时只需要将该部分替换为正式接口即可

详细内容请见注释



#### 5. **GlobalSettings**

该类为静态类，用于设获取全局设置，目前提供以下全局设置信息

1：音乐音量：ActualVolumeOfMusic，只读属性，取值范围为[0,1]的float，直接赋值给AudioSource的volume即可

2：效果音音量：ActualVolumeOfFX，只读属性，取值范围为[0,1]的float，直接赋值给AudioSource的volume即可

目前来讲，只有这两个设置会影响小游戏，请注意在小游戏里完成与音量设置的同步代码



### 1. Hierarchy说明：

打开Scenes/TestField场景，Hierarchy下有若干对象，其中

 1. 所有以**[ENV]**开头的为主场景对象，该场景下使用Layer为Default与UI，使用Sorting Layer为Default，你的小游戏实现不应该和其有任何主动交互与影响，包括但不限于摄像机视野、碰撞检测、光照影响等。小游戏应该不需要关心该环境的内容。

 2. 有两个--开头结尾的对象只是用来当注释信息的，不用管

 3. **MiniGame Camera**：

    此为用于小游戏的主相机

    ​	模式：正交

    ​	尺寸：5.4

    ​	启用Post Processing

    ​	Culling Mask：MiniGame

    ​	Volume Mask：MiniGame

    ​	额外组件：AudioListener与Physics 2D Raycaster

 4. **MiniGameUI Camera**：

    此为用于小游戏UI的主相机，作为MiniGame Camera的Overlay层呈现

    ​	模式：正交

    ​	尺寸：5.4

    ​	Culling Mask：MiniGameUI

    ​	Volume Mask：None

 5. **EventSystem**

    创建UI后自动生成的事件系统承载对象

    

    *注：请勿对上述对象及其子对象进行任何改动（包括添加额外的游戏对象）*

    

6. **GamePrefab**

   小游戏预制体，该预制体默认为SampleGame

   SampleGame为预制到TestField场景的预制体，以该预制体为根节点，其下的所有对象为小游戏的对象。


### 2. 测试自己的小游戏

​	测试前注意事项：

​		1：请将小游戏内所有非UI对象的Layer更改为**MiniGame**

​		2：请将小游戏内所有UI对象的Layer更改为**MiniGameUI**

​		3：请将小游戏内所有可以设置Sorting Layer对象的Sorting Layer更改为**MiniGame**

​		4：制作成Prefab时，请将继承自MiniGameBase的组件挂载到Prefab的根节点上

​		6：请勿对除GamePrefab及其子对象的其他游戏对象进行任何改动（包括添加其他游戏对象的操作）

​		7：小游戏内请勿使用任何会影响全局的设置，包括设置时间缩放、帧率等等

​	将你的小游戏主体部分做成Prefab，然后将该Prefab拖拽到**TestField**中，并将其命名为GamePrefab后即可。

### 3. 测试操作

​	1：主界面提供三个按钮，分别是**进入VN模式游戏**，**关闭游戏**，**进入营地模式游戏**，分别用来模拟主环境载入小游戏和关闭小游戏的操作。主环境顶部用以显示相关的操作提示信息

### 4. 测试标准

​	**1：场景环境无关：**

​		在不修改除你的GamePrefab及其子对象的情况下（换言之，仅修改小游戏相关的内容而不改动任何全局设置），在TestField中正常游玩且不影响主环境（如触发主环境的碰撞检测或引起其他预期外的修改），并正确加载/关闭小游戏以及引发GameCompleted事件即可。（同时避免添加额外的全局信息，包括Layer和Sorting Layer，Tag等信息。但如果确实无法避免添加额外信息，请将其写在README中以便合并时调整。）

​	**2：全局环境无关：**

​	小游戏开启/关闭后不会对游戏全局设置造成任何影响





## BUG与调整

小游戏应不依赖主环境且不与主环境发生任何预期外的交互（光照，碰撞，事件触发等），尽可能通过调整小游戏（如Layer与Sorting Layer）进行独立性调整。

如确实有需要，请在下方填入bug反馈与调整要求

1. NULL