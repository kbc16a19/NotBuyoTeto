﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotBuyoTeto.Ingame.Tetrin {
    public class HoldMino : MonoBehaviour {
        [SerializeField] private TetoSfxManager sfxManager;
        [SerializeField] private MinoResolver resolver;
        [SerializeField] private MinoFrame frame;

        private Animator animator;

        public MinoType? Type { get; private set; }
        public bool Locked { get; private set; }

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        public void Lock() {
            Locked = true;
        }

        public void Free() {
            Locked = false;
        }

        public void Clear() {
            Type = null;
        }

        public bool Push(MinoType type) {
            // ロックされていたら何もしない
            if (Locked) {
                return false;
            }

            // セット
            Type = type;
            frame.Set(type);

            // ホールドをロック
            Lock();

            // SE
            sfxManager.Play(TetoSfxType.MinoHold);

            // アニメーション
            animator.Play(@"HoldAnimation", 0, 0.0f);

            return true;
        }
    }
}