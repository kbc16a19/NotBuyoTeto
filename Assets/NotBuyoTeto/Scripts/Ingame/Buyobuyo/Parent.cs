﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotBuyoTeto.Ingame.Buyobuyo {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Parent : MonoBehaviour {
        private BuyoSfxManager sfxManager;
        private ParticleSystem dropEffect;
        private new Rigidbody2D rigidbody;

        private bool hit = false;

        private float prevHorizontal;
        private static float LimitHorizontalVelocity = 4.2f;
        private static float LimitAngularVelocity = 180.0f;

        private float fallSpeed;
        private float fallAccelaration;
        public int DropFrames { get; private set; }

        private static float SoftdropPeekAccelaration = 6.58f;
        private static float HarddropPeekAccelaration = SoftdropPeekAccelaration * 1.5f;
        private static AnimationCurve DropAccelarationCurve = new AnimationCurve(
            new Keyframe(0, 0, 1.617043f, 1.617043f, 0, 0.1401876f),
            new Keyframe(1, 1, 0.7356746f, 0.7356746f, 0.3873379f, 0)
        );
        private static int DropPeekFrames = 60;

        public void Awake() {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        public Parent Initialize(BuyoSfxManager sfxManager, float fallSpeed) {
            this.fallSpeed = fallSpeed;
            this.sfxManager = sfxManager;
            return this;
        }


        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            var velocity = new Vector2(rigidbody.velocity.x, -fallSpeed);
            var torque = 0.0f;

            var horizontal = Input.GetAxis(@"Horizontal");
            if (prevHorizontal <= 0 && horizontal > 0 || prevHorizontal >= 0 && horizontal < 0) {
                sfxManager.Play(BuyoSfxType.BuyoMove);
            }
            if (horizontal < 0) {
                velocity.x -= 0.122f;
            }
            if (horizontal > 0) {
                velocity.x += 0.122f;
            }
            velocity.x = Mathf.Clamp(velocity.x, -LimitHorizontalVelocity, LimitHorizontalVelocity);
            prevHorizontal = horizontal;

            var vertical = Input.GetAxis(@"Vertical");
            if (vertical != 0) {
                DropFrames++;
                var frames = Mathf.Clamp(DropFrames, 0, DropPeekFrames);
                var peekAccelaration = (vertical < 0) ? SoftdropPeekAccelaration : HarddropPeekAccelaration;
                fallAccelaration = peekAccelaration * DropAccelarationCurve.Evaluate((float)frames / DropPeekFrames);
            }
            else {
                DropFrames = 0;
                fallAccelaration *= 0.86f;
            }

            if (Input.GetButtonDown(@"Rotate Left") || Input.GetButtonDown(@"Rotate Right")) {
                sfxManager.Play(BuyoSfxType.BuyoRotate);
            }
            if (Input.GetButton(@"Rotate Left")) {
                torque += 2.08f;
            }
            if (Input.GetButton(@"Rotate Right")) {
                torque -= 2.08f;
            }

            velocity.y -= fallAccelaration;

            rigidbody.AddTorque(torque);
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = Mathf.Clamp(rigidbody.angularVelocity, -LimitAngularVelocity, LimitAngularVelocity);

            //            dropEffect.transform.rotation = Quaternion.identity;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (hit) { return; }
            if (other.collider.CompareTag(@"Wall")) { return; }
            hit = true;
            GetComponentInChildren<BuyoController>().BuyoHit();
        }
    }
}