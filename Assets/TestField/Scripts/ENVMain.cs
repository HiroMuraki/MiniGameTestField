using UnityEngine;
using UnityEngine.UI;

namespace TestField {
    public class ENVMain : MonoBehaviour {
        public Text MessageDisplayer;
        public Camera MiniGameCamera;
        public Camera MiniGameUICamera;

        void Awake() {
            // 获取对当前小游戏的引用，方便后续操作
            _game = GameObject.Find("GamePrefab")?.GetComponent<MiniGameBase>();
            if (_game == null) {
                throw new System.Exception("未找到小游戏的根节点'GamePrefab'，请确保根节点其命名正确且挂载了MiniGameBase子类");
            }
        }
        void Start() {
            _game.MainCamera = MiniGameCamera;
            _game.UICamera = MiniGameUICamera;
            // 侦听小游戏的GameCompleted事件，触发后显示信息并关闭小游戏
            _game.GameCompleted += (status) => {
                MessageDisplayer.text = $"侦测到小游戏完成并返回状态码{status}";
                CloseGameHelper();
            };
            // 侦听小游戏的CloseRequested事件
            _game.CloseRequested += () => {
                MessageDisplayer.text = "侦测到小游戏请求关闭操作";
                CloseGameHelper();
            };
        }

        public void StartGame(int type) {
            _game.gameObject.SetActive(true);
            if (type == 0) {
                MessageDisplayer.text = $"主环境请求以VN模式载入小游戏";
                _game.LoadGame(() => { }, MiniGameLoadMode.FromVisualNovel);
            }
            else if (type == 1) {
                MessageDisplayer.text = $"主环境请求以营地模式载入小游戏";
                _game.LoadGame(() => { }, MiniGameLoadMode.FromCampsite);
            }
        }
        public void CloseGame() {
            MessageDisplayer.text = $"主环境主动关闭小游戏";
            CloseGameHelper();
        }

        private MiniGameBase _game;
        private void CloseGameHelper() {
            _game.UnloadGame(() => {
                _game.gameObject.SetActive(false);
            });
        }
    }
}
