﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotBuyoTeto.Ingame.Tetrin {
    [RequireComponent(typeof(Collider2D))]
    public class ColliderBlock : MonoBehaviour {
        [SerializeField]
        private GameObject indicator;

        public Collider2D Collider { get; private set; }
        public int EnterCount { get; private set; }

        public bool IsEntered => EnterCount > 0;

        private void Awake() {
            this.Collider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            ++EnterCount;
            if (EnterCount == 1) {
                indicator.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            --EnterCount;
            if (EnterCount == 0) {
                indicator.SetActive(false);
            }
        }
    }
}