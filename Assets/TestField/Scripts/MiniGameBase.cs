﻿using UnityEngine;
using System;

namespace TestField {
    public abstract class MiniGameBase : MonoBehaviour {
        /// <summary>
        /// 小游戏完成
        /// </summary>
        public event Action<int> GameCompleted;
        /// <summary>
        /// 小游戏请求关闭
        /// </summary>
        public event Action CloseRequested;

        /// <summary>
        /// 载入游戏前调用
        /// </summary>
        /// <param name="onCompleted">后续操作回调</param>
        public abstract void LoadGame(Action onCompleted, MiniGameLoadMode loadMode);
        /// <summary>
        /// 卸载游戏时调用
        /// </summary>
        /// <param name="onCompleted">后续操作回调</param>
        public abstract void UnloadGame(Action onCompleted);
        /// <summary>
        /// 引发GameCompleted事件
        /// </summary>
        /// <param name="status">返回的状态码</param>
        public virtual void OnGameCompleted(int status) {
            GameCompleted?.Invoke(status);
        }
        /// <summary>
        /// 引发CloseRequested事件
        /// </summary>
        public virtual void OnCloseRequested() {
            CloseRequested?.Invoke();
        }
    }
}
