using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotBuyoTeto.Ingame.Tetrin {
    [RequireComponent(typeof(Rigidbody2D))]
    public class MinoController : MonoBehaviour {
        private TetoSfxManager sfxManager;
        private ParticleSystem dropEffect;
        private new Rigidbody2D rigidbody;

        private bool hit = false;
        public event EventHandler Hit;

        private MinoControlSettings settings;
        private float prevHorizontal;

        private float fallSpeed;
        private float fallAccelaration;
        public float DropTime { get; private set; }
        public float ReleaseTime { get; private set; }

        public void Awake() {
//            dropEffect = GetComponentInChildren<ParticleSystem>();
            rigidbody = GetComponent<Rigidbody2D>();
        }

        public MinoController Initialize(TetoSfxManager sfxManager, MinoControlSettings settings, float fallSpeed) {
            this.sfxManager = sfxManager;
            this.settings = settings;
            this.fallSpeed = fallSpeed;
            return this;
        }

        public void Start() {
//            dropEffect.Play();
        }

        public void Update() {
            var velocity = new Vector2(rigidbody.velocity.x, -fallSpeed);
            var torque = 0.0f;

            var horizontal = Input.GetAxis(@"Horizontal");
            if (prevHorizontal <= 0 && horizontal > 0 || prevHorizontal >= 0 && horizontal < 0) {
                sfxManager.Play(TetoSfxType.MinoMove);
            }
            if (horizontal < 0) {
                velocity.x -= settings.HorizontalVelocity * Time.deltaTime;
            }
            if (horizontal > 0) {
                velocity.x += settings.HorizontalVelocity * Time.deltaTime;
            }
            velocity.x = Mathf.Clamp(velocity.x, -settings.LimitHorizontalVelocity, settings.LimitHorizontalVelocity);
            prevHorizontal = horizontal;

            var vertical = Input.GetAxis(@"Vertical");
            if (vertical != 0) {
                var t1 = Mathf.Clamp(DropTime, 0, settings.AccelarationTime);
                var t2 = t1 / settings.AccelarationTime;
                var t3 = settings.AccelarationCurve.Evaluate(t2);
                var peek = (vertical < 0) ? settings.SoftdropPeekAccelaration : settings.HarddropPeekAccelaration;
                fallAccelaration = peek * t3;

                DropTime += Time.deltaTime;
                ReleaseTime = 0.0f;
            } else {
                var t1 = Mathf.Clamp(ReleaseTime, 0, settings.DecelerationTime);
                var t2 = t1 / settings.DecelerationTime;
                var t3 = settings.DecelerationCurve.Evaluate(t2);
                fallAccelaration = Mathf.Lerp(fallAccelaration, 0.0f, t3);

                ReleaseTime += Time.deltaTime;
                DropTime = 0.0f;
            }

            if (Input.GetButtonDown(@"Rotate Left") || Input.GetButtonDown(@"Rotate Right")) {
                sfxManager.Play(TetoSfxType.MinoRotate);
            }
            if (Input.GetButton(@"Rotate Left")) {
                torque += settings.AngularVelocity * Time.deltaTime;
            }
            if (Input.GetButton(@"Rotate Right")) {
                torque -= settings.AngularVelocity * Time.deltaTime;
            }

            velocity.y -= fallAccelaration;

            rigidbody.AddTorque(torque);
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = Mathf.Clamp(rigidbody.angularVelocity, -settings.LimitAngularVelocity, settings.LimitAngularVelocity);

//            dropEffect.transform.rotation = Quaternion.identity;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (hit) { return; }
            if (other.collider.CompareTag(@"Wall")) { return; }

            hit = true;
            sfxManager.Play(TetoSfxType.MinoHit);

//            dropEffect.Stop();
//            Destroy(dropEffect.gameObject, 1.0f);
            Destroy(this);

            Hit?.Invoke(this, EventArgs.Empty);
        }
    }
}
