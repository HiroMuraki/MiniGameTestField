using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TestField {
    public class SampleGame : MiniGameBase {
        public Text MessageDisplayer;
        public GameObject Ball;

        void Start() {
            // 侦听所有Wall的碰撞事件，当小球碰撞到墙后引发GameCompleted事件，并简单地将墙的ID作为返回码
            foreach (var wall in transform.Find("Walls").GetComponentsInChildren<Wall>()) {
                wall.WallCollided += (wallID) => {
                    OnGameCompleted(wallID);
                };
            }
        }
        void Update() {
            // 游戏暂停时不检测做任何操作
            if (!_gameAcitved) {
                return;
            }

            // !!! 如果当前模式为VN模式，按下空格时显示/跳过/隐藏对话
            // !!! 此部分用于演示在你的小游戏中如何根据从LoadGame方法中获取的MiniGameLoadMode的值调整游戏具体行为
            if (_gameMode == MiniGameLoadMode.FromVisualNovel && Input.GetKeyDown(KeyCode.Space)) {
                // 如果当前没有对话，则显示一条对话
                if (!_hasDialog) {
                    VNSimulation.Current.ShowDialog("阿草", "小伙伴你好！建议收到！", "阿草-通常");
                    _hasDialog = true;
                }
                else {
                    // 否则如果对话的打字机动画未完成，此次点击视为跳过动画；否则视为隐藏对话
                    if (!VNSimulation.Current.IsDialogTextAnimationCompleted()) {
                        VNSimulation.Current.SkipDialogTextAnimation();
                    }
                    else {
                        VNSimulation.Current.HideDialog();
                        _hasDialog = false;
                    }
                }
            }

            // 没什么特殊操作，就是AD移动小球，实现也很简单粗暴，不用管
            if (Input.GetKey(KeyCode.A)) {
                _targetPos = Ball.transform.position;
                _targetPos.x -= 1.0f;
            }
            else if (Input.GetKey(KeyCode.D)) {
                _targetPos = Ball.transform.position;
                _targetPos.x += 1.0f;
            }
            if (Vector3.SqrMagnitude(_targetPos - Ball.transform.position) >= 0.0001) {
                Ball.transform.position = Vector3.Lerp(Ball.transform.position, _targetPos, 10 * Time.deltaTime);
            }
        }


        /// <summary>
        /// !!! 此方法为此示例小游戏对MiniGameBase中抽象LoadGame方法的实现
        /// </summary>
        /// <param name="onCompleted">加操作完成后执行的回调></param>
        /// <param name="loadMode">小游戏的载入模式</param>
        public override void LoadGame(Action onCompleted, MiniGameLoadMode loadMode) {
            // 此类中有一个字段用于储存当前的游戏载入模式（因为在其他地方需要根据此字段的值调整行为）
            _gameMode = loadMode;
            // 设置UI的Camera
            transform.Find("MiniGame UI").GetComponent<Canvas>().worldCamera = UICamera;

            // !!! 仅作为简单示例，如果是FromVisualNovel模式，则载入时小球为蓝色，从营地载入时小球为红色
            // !!! 在实际实现中，你的小游戏应该需要根据loadMode的值禁用/启用某些操作，如设置/排行榜/关闭/难度选择等
            switch (_gameMode) {
                case MiniGameLoadMode.FromVisualNovel:
                    Ball.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.blue;
                    MessageDisplayer.text = "小游戏已从VN中载入，小球当前为蓝色";
                    break;
                case MiniGameLoadMode.FromCampsite:
                    Ball.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.red;
                    MessageDisplayer.text = "小游戏已从营地中载入，小球当前为红色";
                    break;
            }

            // !!! 下面是初始化游戏，由于游戏比较简单，因此只需要重置小球的位置即可
            Ball.transform.position = Vector2.zero;
            _targetPos = Vector2.zero;

            // !!! 调用onCompleted回调函数，让其完成后续操作
            onCompleted?.Invoke();

            // 等待onCompleted完成操作，因此_gameActived在onCompleted完成后才设置为true
            _gameAcitved = true;
        }

        /// <summary>
        /// !!! 此方法为此示例小游戏对MiniGameBase中抽象UnloadGame方法的实现
        /// </summary>
        /// <param name="onCompleted">卸载操作完成后执行的回调</param>
        public override void UnloadGame(Action onCompleted) {
            _gameAcitved = false;

            // !!! 下面是重置游戏状态，由于游戏比较简单，因此只需要重置小球的位置即可
            Ball.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.white;
            Ball.transform.position = Vector2.zero;
            _targetPos = Vector2.zero;
            MessageDisplayer.text = "小游戏已关闭，小球当前为白色";

            // !!! 调用onCompleted回调函数，让其完成后续操作
            onCompleted?.Invoke();
        }

        private MiniGameLoadMode _gameMode; // 储存从LoadGame方法中获取的游戏载入模式
        private Vector3 _targetPos;
        private bool _gameAcitved;
        private bool _hasDialog;
    }
}
