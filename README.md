# Mini Game Test Field

## 说明

该项目为小游戏测试场地，用于确保小游戏的兼容性

说人话就是只要你的小游戏能在这里正常跑，基本上塞到主游戏里面就没什么问题了



## 开发信息

1. Unity版本：2019.4.32f1
2. 使用管线：Universal Render Pipeline（URP）
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

     * **MiniGame**：小游戏相关文件夹
       * Scripts：示例小游戏相关文件夹
         * SampleGame.cs：示例小游戏的主代码
         * Wall.cs：示例小游戏的墙体组件
       * Images：图片资源
       * Prefabs：小游戏的预制体




## 使用说明

### 0. MiniGameBase基类说明

所有事件/方法均为public

后续接口可能有修改，但大致上应该不会有太大变化

```c#
/* 该事件表示小游戏已完成，在游戏完成时通过OnGameCompleted方法引发该事件，并附带状态码
   具体的后续操作由事件注册者决定*/
event Action<int> GameCompleted;


/* 该事件表示小游戏发起关闭请求，在游戏内关闭游戏小游戏通过OnCloseRequested方法引发该事件
   具体的关闭操作由于事件注册者决定*/
event Action CloseRequested;


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

​	（如果你的小游戏确实需要多个Layer或者Sorting Layer，请将其写在README中）

​	将你的小游戏主体部分做成Prefab，然后将该Prefab拖拽到**TestField**中，并将其命名为GamePrefab后即可。

### 3. 测试操作

​	1：主界面提供三个按钮，分别是**进入VN模式游戏**，**关闭游戏**，**进入营地模式游戏**，分别用来模拟主环境载入小游戏和关闭小游戏的操作。主环境顶部用以显示相关的操作提示信息

### 4. 测试标准

在不修改除你的GamePrefab及其子对象的情况下，在TestField中正常游玩且不影响主环境（如触发主环境的碰撞检测或引起其他预期外的修改），并正确加载/关闭小游戏以及引发GameCompleted事件即可。



## BUG与调整

小游戏应不依赖主环境且不与主环境发生任何预期外的交互（光照，碰撞，事件触发等），尽可能通过调整小游戏（如Layer与Sorting Layer）进行独立性调整。

如确实有需要，请在下方填入bug反馈与调整要求

```
NULL
```