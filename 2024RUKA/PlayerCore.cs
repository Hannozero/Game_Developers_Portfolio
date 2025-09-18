/*
 * 주의! 해당스크립트는 최소 3명의 이상의 프로그래머가 같이 작업한 스크립트입니다!
 * 포트폴리오를 작성한 본인이 작성한 코드만 적힌 상태이며,
 * 다른 프로그래머가 작성한 코드는 삭제했습니다!
 */


    private void FixedUpdate()
    {


        //이전 프레임의 플레이어 속도
        Vector3 currentVelocity = rBody.velocity;

        // 이전 프레임과 현재 프레임의 속도를 비교하여 속도의 변화를 확인합니다.
        Vector3 velocityChange = currentVelocity - previousVelocity;

        // 1프레임 전의 속도를 출력합니다.
        //Debug.Log("1프레임 전의 속도: " + previousVelocity.magnitude);

        // 현재 프레임의 속도를 이전 프레임의 속도로 업데이트합니다.
        previousVelocity = currentVelocity;
    }


    #region ================ MovementStates ================

    //============================================
    //
    // MovementStates는 플레이어의 현재 행동을 나타내는 state패턴의 클래스들 입니다.
    // CurrentMovement를 통해 현재 플레이어의 움직임 state를 변경할 수 있습니다.
    // CurrentMovement가 바뀌면 이전 state의 OnMovementExit가 호출되고 바뀔 state의 OnMovementEnter가 호출됩니다.
    //
    //============================================

    protected class MovementState
    {
        /// <summary>
        /// 해당 state로 들어올 때 이 함수가 호출됩니다.
        /// </summary>
        /// <param name="player"> 플레이어 인스턴스 </param>
        public virtual void OnMovementEnter(PlayerCore @player) { }
        /// <summary>
        /// Update 루프 때 이 함수가 호출됩니다
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnUpdate(PlayerCore @player) { }
        /// <summary>
        /// FixedUpdate 루프 때 이 함수가 호출됩니다.
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnFixedUpdate(PlayerCore @player) { }
        /// <summary>
        /// 해당 state에서 나가라 때 이 함수가 호출됩니다.
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnMovementExit(PlayerCore @player) { }
    }

    float slopeResistence = 1f;


/// <summary>
/// 플레이어가 땅 위를 뛰어다니는 상태일 때
/// </summary>
    protected class Movement_Ground : MovementState
    {
        bool sliding = false;
        float idleAnimationtime = 30f;
        float IdleTimer = 0f;
        private float GetSlopeForwardInterpolation(PlayerCore player,Vector3 forward)
        {
            return Mathf.InverseLerp(player.slopeEffect.x / 90f, player.slopeEffect.y / 90f, Vector3.Dot(forward, Vector3.up));
        }

        public override void OnMovementEnter(PlayerCore player)
        {
            player.movementStateRefernce = "Ground";
        }

        public override void OnFixedUpdate(PlayerCore player)
        {
            base.OnFixedUpdate(player);

            if (player.buoyant.SubmergeRateZeroClamped < 0)
            {
                player.rBody.AddForce(Vector3.up * player.swimUpforce, ForceMode.Acceleration);
            }

            if(sliding)
            {

            }
            else if (player.input.Player.Move.IsPressed())
            {
                //forward velocity
                Vector3 lookTransformedVector = player.GetLookMoveVector(player.input.Player.Move.ReadValue<Vector2>(), Vector3.up);
                Vector3 slopedMoveVector = Vector3.ProjectOnPlane(lookTransformedVector, player.groundNormal).normalized;
                //player.slopeResistence = 1f - Mathf.InverseLerp(player.slopeEffect.x / 90f, player.slopeEffect.y / 90f,Vector3.Dot(slopedMoveVector, Vector3.up));

                float adjuestedScale = (player.sprinting && player.grounding) ? player.sprintSpeedMult : 1.0f;

                Vector3 finalVelocity = slopedMoveVector * adjuestedScale * player.slopeResistence * player.FinalMoveSpeed * ((player.currentHoldingItem == null)?1.0f:player.holdingMoveSpeedMult);
                if (player.buoyant.WaterDetected)
                {
                    finalVelocity = finalVelocity * (1f - Mathf.Lerp(0.5f, 0f, player.buoyant.SubmergeRate) * player.waterWalkDragging);
                }

                player.rBody.velocity = new Vector3(finalVelocity.x, player.rBody.velocity.y, finalVelocity.z);

                bool LargeTurn = Quaternion.Angle(player.transform.rotation, Quaternion.LookRotation(lookTransformedVector, Vector3.up)) > 60f;

                player.transform.rotation = Quaternion.RotateTowards(
                    player.transform.rotation,
                    Quaternion.LookRotation(lookTransformedVector, Vector3.up),
                    LargeTurn ? 30f : 10f
                );

                if (player.buoyant.WaterDetected)
                    player.animator.speed = 1f - Mathf.Lerp(0.5f, 0f, player.buoyant.SubmergeRate) * player.waterWalkDragging;
                else 
                    player.animator.speed = 1.0f;

                player.animator.SetBool("MovementInput", true);

                IdleTimer = 0f;
            }
            else
            {
                player.rBody.velocity = Vector3.Lerp(player.rBody.velocity, new Vector3(0f, player.rBody.velocity.y, 0f), player.horizontalDrag / 0.2f);
                player.animator.SetBool("MovementInput", false);

                IdleTimer += Time.fixedDeltaTime;

                if (IdleTimer > idleAnimationtime)
                {
                    player.animator.SetTrigger("IdleAnimation");
                    IdleTimer = 0f;
                    idleAnimationtime = Random.Range(25f, 40f);
                }

            }
        }

        public override void OnUpdate(PlayerCore player)
        {
            base.OnUpdate(player);
            if (player.sprinting) player.animator.SetFloat("RunBlend", 1f, 0.1f, Time.deltaTime);
            else player.animator.SetFloat("RunBlend", 0f, 0.1f, Time.deltaTime);
        }

        public override void OnMovementExit(PlayerCore player)
        {
            base.OnMovementExit(player);
            player.ReleaseHoldingItem();
        }
    }

    readonly float waterjumpInterval = 1f;
    float waterjumpTimer = 0f;

    /// <summary>
    /// 플레이어가 수영중인 상황일 때
    /// </summary>
    protected class Movement_Swimming : MovementState
    {
        public override void OnMovementEnter(PlayerCore player)
        {
            player.rBody.drag = player.swimRigidbodyDrag;
            base.OnMovementEnter(player);
            player.animator.SetBool("Swimming", true);
            player.animator.SetTrigger("SwimmingEnter");
            player.movementStateRefernce = "Swimming";
        }

        public override void OnFixedUpdate(PlayerCore player)
        {
            base.OnFixedUpdate(player);
            player.waterjumpTimer += Time.fixedDeltaTime;

            if (player.buoyant.SubmergeRateZeroClamped < 0)
            {
                player.rBody.AddForce(Vector3.up * player.swimUpforce * (0.5f + Mathf.Sin(Time.time) / 2f));
            }

            if (player.input.Player.Move.IsPressed())
            {
                Vector3 lookTransformedVector = player.GetLookMoveVector(player.input.Player.Move.ReadValue<Vector2>(), Vector3.up);

                Vector3 finalVelocity = lookTransformedVector * player.swimSpeed;
                player.rBody.velocity = new Vector3(finalVelocity.x, player.rBody.velocity.y, finalVelocity.z);

                player.transform.rotation = Quaternion.RotateTowards(
                    player.transform.rotation,
                    Quaternion.LookRotation(lookTransformedVector, Vector3.up),
                    5f
                );
                player.animator.SetBool("Swimming_Move", true);
            }
            else
            {
                player.rBody.velocity = Vector3.Lerp(player.rBody.velocity, new Vector3(0, player.rBody.velocity.y, 0f), player.horizontalDrag);
                player.animator.SetBool("Swimming_Move", false);
            }
        }

        public override void OnMovementExit(PlayerCore player)
        {
            player.animator.SetBool("Swimming", false);
            player.rBody.drag = player.initialRigidbodyDrag;
            base.OnMovementExit(player);
        }
    }

    /// <summary>
    /// 플레이어가 조각배를 타는 상황일 때
    /// </summary>
    protected class Movement_Sailboat : MovementState
    {
        Vector3 directionCache;
        float GustAmount = 0.0f;
        bool enterFlag = false;
        float driftAngle = 0f;
        float driftAngleMax = 90f;
        float driftTime = 0f;
        bool driftChargeFlag = false;
        Vector3 driftDirection = Vector3.zero;

        public override void OnMovementEnter(PlayerCore player)
        {
            directionCache = player.transform.forward;
            base.OnMovementEnter(player);
            player.sailboat.gameObject.SetActive(true);
            player.sailboatEngineSound.EventInstance.setParameterByName("SailboatEngine", 0f);
            player.sailboatFootRig.weight = 1.0f;
            player.buoyant.enabled = false;
            player.rBody.useGravity = false;
            player.animator.SetBool("Boarding", true);
            player.animator.SetTrigger("BoardingEnter");
            player.animator.SetFloat("BoardBlend", 0.0f);
            UI_SailboatSkillInfo.Instance.ToggleInfo(true);
            player.movementStateRefernce = "Sailboat";
        }

        private Vector3 GetSailboatHeadingVector(PlayerCore player, Vector3 input, Vector3 up)
        {

            Vector3 lookTransformedVector;

            if (player.boosterActive)
            {
                lookTransformedVector = Vector3.RotateTowards(player.transform.forward, Camera.main.transform.TransformDirection(new Vector3(input.x, 0f, 1.0f)), player.FinalSteering, 1.0f);
            }
            if (player.DriftActive)
            {
                lookTransformedVector = Vector3.RotateTowards(player.transform.forward, Camera.main.transform.TransformDirection(new Vector3(input.x, 0f, Mathf.Clamp(input.y, 0.5f, 1.0f))), player.FinalSteering, 1.0f);
            }
            else
            {
                lookTransformedVector = Vector3.RotateTowards(player.transform.forward, Camera.main.transform.TransformDirection(new Vector3(input.x, 0f, input.y)), player.FinalSteering, 1.0f);
            }

            //}
            lookTransformedVector = Vector3.ProjectOnPlane(lookTransformedVector, up);
            return lookTransformedVector;
        }
        
        public override void OnUpdate(PlayerCore player)
        {
            if (player.input.Player.SailboatDrift.WasPressedThisFrame() && player.sailboat.SubmergeRate < 5.0f &&player.input.Player.Move.ReadValue<Vector2>().x != 0)
            {
                //player.driftActive = true;
                driftDirection = new Vector3(player.input.Player.Move.ReadValue<Vector2>().x > 0 ? 1f : -1,0f,0f);
            }

            if (player.driftActive && player.input.Player.SailboatDrift.WasReleasedThisFrame())
            {
                player.driftActive = false;

                if (driftTime > player.driftKickRequireingTime)
                {
                    player.rBody.AddForce(player.sailboatModelPivot.forward * player.driftKickPower, ForceMode.VelocityChange);
                    RuntimeManager.PlayOneShot(player.sound_driftKick);
                }
                driftTime = 0f;
                driftChargeFlag = false;
            }

            Vector2 moveInput = player.input.Player.Move.ReadValue<Vector2>();

            if (player.driftActive)
            {
                if(moveInput.x > 0)
                {
                    driftDirection = new Vector3(1f, 0f, 0f);
                }
                else if(moveInput.x < 0)
                {
                    driftDirection = new Vector3(-1f, 0f, 0f);
                }

                float f;


                driftAngle = Mathf.Lerp(driftAngle, driftAngleMax * moveInput.x, player.driftSteer);
                driftTime += Time.deltaTime;

                if(driftTime > player.driftKickRequireingTime)
                {
                    if (!driftChargeFlag)
                    {
                        driftChargeFlag = true;
                        RuntimeManager.PlayOneShot(player.sound_driftCharged);
                    }

                    player.driftSound.EventInstance.getParameterByName("Drift", out f);
                    player.driftSound.EventInstance.setParameterByName("Drift", Mathf.Lerp(f, 1.0f, 0.1f));
                }
                else
                {
                    player.driftSound.EventInstance.getParameterByName("Drift", out f);
                    player.driftSound.EventInstance.setParameterByName("Drift", Mathf.Lerp(f, 0.5f, 0.1f));
                }
            }
            else
            {
                float f;
                player.driftSound.EventInstance.getParameterByName("Drift", out f);

                player.driftSound.EventInstance.setParameterByName("Drift", Mathf.Lerp(f, 0.0f, 0.1f));

                driftAngle = Mathf.Lerp(driftAngle, 0f, player.driftSteer);
            }

            player.animator.SetFloat("Board_X", moveInput.x, 0.3f, Time.deltaTime);
            player.animator.SetFloat("Board_Y", moveInput.y, 0.3f, Time.deltaTime);

            if (player.sailboat.SubmergeRate < player.leapupAvailHeight)
            {
                UI_SailboatSkillInfo.Instance.SetLeapupAvailable(true);
            }
            else
            {
                UI_SailboatSkillInfo.Instance.SetLeapupAvailable(false);
            }

        }

        public override void OnFixedUpdate(PlayerCore player)
        {
            base.OnFixedUpdate(player);

            SailboatBehavior sailboat = player.sailboat;
            GustAmount = Mathf.InverseLerp(player.gustStartVelocity, player.gustMaxVelocity, Vector3.ProjectOnPlane(player.rBody.velocity, Vector3.up).magnitude);

            float ns_boost = sailboat.SubmergeRate < player.sailboatNearsurf && sailboat.SubmergeRate > -0.5f ? player.sailboatNearsurfBoost : 1.0f;

            Vector2 moveInput = player.input.Player.Move.ReadValue<Vector2>();


            if (player.sailboat.SubmergeRate < -1.5f)
            {
                player.rBody.drag = player.sailboatFullDrag;
                player.rBody.AddForce(Vector3.up * -Mathf.Clamp(sailboat.SubmergeRate, -5.0f, 0.0f) / 3f * player.sailboatByouancy, ForceMode.Acceleration);

                Vector3 lookTransformedVector;

                lookTransformedVector = GetSailboatHeadingVector(player, moveInput, player.sailboat.SurfacePlane.normal);

                player.rBody.AddForce(lookTransformedVector * player.FinalSailboatAcceleration);

            }
            else if (player.sailboat.SubmergeRate < 0.5f)
            {
                player.rBody.drag = player.sailboatScratchDrag;
                player.rBody.AddForce(Vector3.up * -Mathf.Clamp(sailboat.SubmergeRate, -1.0f, 0.0f) * player.sailboatByouancy, ForceMode.Acceleration);
                player.rBody.AddForce(Vector3.ProjectOnPlane(sailboat.SurfacePlane.normal, Vector3.up) * player.sailboatSlopeInfluenceForce, ForceMode.Acceleration);

                Vector3 lookTransformedVector;
                lookTransformedVector = GetSailboatHeadingVector(player, moveInput, player.sailboat.SurfacePlane.normal);

                player.rBody.AddForce(lookTransformedVector * player.FinalSailboatAcceleration * ns_boost, ForceMode.Acceleration);

                if (!enterFlag)
                {
                    enterFlag = true;
                    if (player.rBody.velocity.y < -1f)
                    {
                        RuntimeManager.PlayOneShot(player.sound_splash);
                        if (player.rBody.velocity.ProjectOntoPlane(Vector3.up).magnitude > 10f)
                        {
                            if (!player.sailingSplashEffect_HighVel.isPlaying)
                                player.sailingSplashEffect_HighVel.Play(true);
                        }
                        else
                        {
                            if (GlobalOceanManager.IsInstanceValid)
                            {
                                Instantiate(player.normalSplashEffectPrefab, new Vector3(player.transform.position.x, GlobalOceanManager.Instance.GetWaveHeight(player.transform.position), player.transform.position.z), Quaternion.identity);
                            }
                        }
                    }
                }
            }
            else
            {
                enterFlag = false;

                if (!player.Grounding)
                {
                    player.rBody.drag = player.sailboatGlidingDrag;

                    Vector3 lookTransformedVector;
                    //if (player.boosterActive)
                    //    lookTransformedVector = player.GetLookMoveVector(new Vector2(moveInput.x, 1f), Vector3.up);
                    //else if (player.driftActive)
                    //    lookTransformedVector = player.GetLookMoveVector(new Vector2(moveInput.x, Mathf.Clamp(moveInput.y, 0.5f, 1.0f)), Vector3.up);
                    //else
                    //    lookTransformedVector = player.GetLookMoveVector(moveInput, Vector3.up);

                    lookTransformedVector = GetSailboatHeadingVector(player, moveInput, Vector3.up);

                    player.rBody.AddForce(lookTransformedVector * player.FinalSailboatAcceleration * ns_boost, ForceMode.Acceleration);
                }


                player.rBody.AddForce(Vector3.up * -Mathf.Clamp(sailboat.SubmergeRate, 0f, 1f) * player.sailboatGravity, ForceMode.Acceleration);
            }


            if (Vector3.ProjectOnPlane(player.rBody.velocity, Vector3.up).magnitude > 5.0f)
            {
                Vector3 pivotEuler = Vector3.zero;

                if (player.input.Player.SailboatForward.IsPressed())
                {
                    if (!player.driftActive)
                    {
                        player.rBody.AddForce(Vector3.up * player.sailboatVerticalControl);

                        pivotEuler = new Vector3(-35f, 0f, 0f);
                    }
                }
                else if (player.input.Player.SailboatBackward.IsPressed())
                {
                    if (!player.driftActive)
                    {
                        player.rBody.AddForce(Vector3.down * player.sailboatVerticalControl);

                        pivotEuler = new Vector3(10f, 0f, 0f);
                    }
                }
                else
                {
                    pivotEuler = new Vector3(0f, 0f, 0f);
                }

                if (player.input.Player.Move.IsPressed())
                {
                    sailboat.transform.rotation = Quaternion.Slerp(sailboat.transform.rotation,
                        Quaternion.LookRotation(player.rBody.velocity, sailboat.SurfacePlane.normal),
                        0.1f);

                    Vector3 lookTransformedVector = player.GetLookMoveVector(player.input.Player.Move.ReadValue<Vector2>(), Vector3.up);
                    if (player.driftActive) lookTransformedVector = GetSailboatHeadingVector(player,driftDirection, Vector3.up);
                    float lean = Vector3.Dot(lookTransformedVector, player.transform.right);
                    if (player.driftActive) lean = lean * 1.5f;

                    pivotEuler = pivotEuler + new Vector3(0f, 0f, -lean * 30f);

                    directionCache = Vector3.ProjectOnPlane(player.rBody.velocity, Vector3.up);
                }
                else
                {
                    sailboat.transform.rotation = Quaternion.LookRotation(directionCache, sailboat.SurfacePlane.normal);
                }

                pivotEuler = pivotEuler + new Vector3(0f,driftAngle,0f);

                player.sailboatModelPivot.localRotation = Quaternion.Slerp(player.sailboatModelPivot.localRotation, Quaternion.Euler(pivotEuler), 0.1f);
            }
            else
            {
                sailboat.transform.rotation = Quaternion.LookRotation(directionCache, sailboat.SurfacePlane.normal);
            }


            player.transform.forward = Vector3.ProjectOnPlane(sailboat.transform.forward, Vector3.up);


            if (player.rBody.velocity.magnitude > 10f && sailboat.SubmergeRate < 1f)
            {
                Vector3 pos = player.sailingSprayEffect.transform.position;
                float surfaceHeight = GlobalOceanManager.Instance.GetWaveHeight(pos);
                player.sailingSprayEffect.transform.position = new Vector3(pos.x,surfaceHeight,pos.z);
                player.sailingSprayEffect.transform.rotation.SetLookRotation(directionCache, sailboat.SurfacePlane.normal);


                if (!player.sailingSprayEffect.isPlaying)
                {
                    player.sailingSprayEffect.Play(true);
                }
            }
            else
            {
                if (player.sailingSprayEffect.isPlaying)
                    player.sailingSprayEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            }

            //Vector3 wavePosition = player.sailingFrontwaveEffect.transform.position;
            //if (player.rBody.velocity.magnitude > 10f && sailboat.SubmergeRate < 1.0f)
            //{
            //    player.sailingFrontwaveEffect.Play(true);
            //    player.sailingFrontwaveEffect.transform.position = new Vector3(wavePosition.x, GlobalOceanManager.Instance.GetWaveHeight(wavePosition), wavePosition.z);
            //    player.sailingFrontwaveEffect.transform.up = player.sailboat.SurfacePlane.normal;
            //    player.sailingFrontwaveEffect.transform.forward = player.transform.forward;
            //}
            //else
            //{
            //    player.sailingFrontwaveEffect.Play(false);
            //    player.sailingFrontwaveEffect.transform.position = new Vector3(wavePosition.x, GlobalOceanManager.Instance.GetWaveHeight(wavePosition), wavePosition.z);
            //    player.sailingFrontwaveEffect.transform.up = player.sailboat.SurfacePlane.normal;
            //    player.sailingFrontwaveEffect.transform.forward = player.transform.forward;
            //}

            player.animator.SetFloat("BoardBlend", player.rBody.velocity.y);

            float value = player.sailboatEngineSound.Params[0].Value;

            if (player.input.Player.Move.IsPressed())
            {
                if (player.boosterActive)
                    player.sailboatEngineSound.EventInstance.setParameterByName("SailboatEngine", 1f);
                else
                    player.sailboatEngineSound.EventInstance.setParameterByName("SailboatEngine", Mathf.Clamp(player.rBody.velocity.magnitude / 40f, 0f, 0.8f));

                player.animator.SetFloat("BoardPropellingBlend", 1f, 1f, Time.fixedDeltaTime);
            }
            else
            {
                player.sailboatEngineSound.EventInstance.setParameterByName("SailboatEngine", 0f);
                player.animator.SetFloat("BoardPropellingBlend", 0f, 1f, Time.fixedDeltaTime);
            }

            player.waterScratchSound.EventInstance.setParameterByName("BoardWaterScratch", Mathf.InverseLerp(0.5f, -0.5f, player.sailboat.SubmergeRate) * GustAmount * 1.5f);

            var em = player.sailingSwooshEffect.emission;
            em.rateOverTimeMultiplier = GustAmount * 3f;

            player.gustSound.EventInstance.setParameterByName("Speed", GustAmount);
        }

        public override void OnMovementExit(PlayerCore player)
        {
            base.OnMovementExit(player);
            player.AbortBooster();
            player.sailingSprayEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            player.sailboat.gameObject.SetActive(false);
            player.sailboatFootRig.weight = 0.0f;
            player.buoyant.enabled = true;
            player.rBody.useGravity = true;
            player.driftActive = false;
            player.rBody.drag = player.initialRigidbodyDrag;
            player.animator.SetBool("Boarding", false);
            player.animator.SetFloat("BoardPropellingBlend", 0f);
            player.animator.SetFloat("Board_X", 0f);
            player.animator.SetFloat("Board_Y", 0f);
            player.driftSound.EventInstance.setParameterByName("Drift", 0f);
            UI_SailboatSkillInfo.Instance.ToggleInfo(false);
            UI_SailboatSkillInfo.Instance.SetLeapupAvailable(true);

            var em = player.sailingSwooshEffect.emission;
            em.rateOverTimeMultiplier = 0f;
        }
    }

    #endregion
    /// <summary>
    /// 현재 플레이어 Movestate를 즉시 바꿉니다.
    /// </summary>
    public void SetMovementState(PlayerMovementState state)
    {
        if (state == PlayerMovementState.Ground) CurrentMovement = new Movement_Ground();
        else if (state == PlayerMovementState.Swimming) CurrentMovement = new Movement_Swimming();
        else if (state == PlayerMovementState.Sailboat) CurrentMovement = new Movement_Sailboat();
    }

    /// <summary>
    /// 현재 플레이어 Movestate를 즉시 바꿉니다. (인덱스)
    /// </summary>
    /// <param name="state"></param>
    public void SetMovementState(int state)
    {
        if (state == (int)PlayerMovementState.Ground) CurrentMovement = new Movement_Ground();
        else if (state == (int)PlayerMovementState.Swimming) CurrentMovement = new Movement_Swimming();
        else if (state == (int)PlayerMovementState.Sailboat) CurrentMovement = new Movement_Sailboat();
    }


    int disableStack = 0;
    public int DisableStack { get { return disableStack; } }

    /// <summary>
    ///  시퀀스 시작시 플레이어의 조작을 비활성화하기 위한 함수.
    /// </summary>
    public void DisableControls()
    {
        if(disableStack <= 0)
        {
            input.Player.Disable();
            Cinemachine.CinemachineInputProvider cameraInputProvider = FindFirstObjectByType<Cinemachine.CinemachineInputProvider>();
            if (cameraInputProvider != null) { cameraInputProvider.enabled = false; }
            if (CurrentMovement.GetType() == typeof(Movement_Sailboat)) UI_SailboatSkillInfo.Instance.ToggleInfo(false);
        }

        disableStack++;
    }

    /// <summary>
    /// 시퀀스 종료시 플레이어의 조작을 활성화하기 위한 함수.
    /// </summary>
    public void EnableControls()
    {
        disableStack--;

        if (disableStack <= 0)
        {
            input.Player.Enable();
            Cinemachine.CinemachineInputProvider cameraInputProvider = FindFirstObjectByType<Cinemachine.CinemachineInputProvider>();
            if (cameraInputProvider != null) { cameraInputProvider.enabled = true; }
            if (CurrentMovement.GetType() == typeof(Movement_Sailboat)) UI_SailboatSkillInfo.Instance.ToggleInfo(true);
            disableStack = 0;
        }
    }

    private Vector3 GetLookMoveVector(Vector2 input, Vector3 up)
    {
        Vector3 lookTransformedVector;

        lookTransformedVector = Camera.main.transform.TransformDirection(new Vector3(input.x, 0f, input.y));

        lookTransformedVector = Vector3.ProjectOnPlane(lookTransformedVector, up).normalized;
        return lookTransformedVector;
    }

    /// <summary>
    /// DropItemCrash가 플레이어 변수에 접근하기 위한 함수
    /// </summary>>
    public void DropItemCrash(float addMoveSpeed, float addSprintSpeed, float addSwimSpeed, float addJumpPower, float addBoatSpeed)
    {
        moveSpeed += addMoveSpeed;
        sprintSpeedMult += addSprintSpeed;
        swimSpeed += addSwimSpeed;
        jumpPower += addJumpPower;
        sailboatAccelerationForce += addBoatSpeed;
    }

    /// <summary>
    /// 플레이어의 업그레이드 함수관리를 하는 변수
    /// </summary>
    
    public void PlayerUpgradeState(AbilityAttribute Type ,float UpgradeState)
    {

        if(Type == AbilityAttribute.MoveSpeed)
        {
            moveSpeed += UpgradeState;
        }

        if(Type == AbilityAttribute.JumpPower)
        {
            jumpPower += UpgradeState;
        }

        if(Type == AbilityAttribute.LeapupPower)
        {
            leapupPower += UpgradeState;
        }
        
        if(Type == AbilityAttribute.BoosterDuration)
        {
            boosterDuration += UpgradeState;
        }

        if(Type == AbilityAttribute.BoosterMult)
        {
            boosterMult += UpgradeState;
        }


    }

    public float ViewJumpPower{get{return jumpPower;}}
    public float ViewMoveSpeed{get{return moveSpeed;}}
    public float ViewleapupPower {get {return FinalLeapupPower;}}
    public float ViewBoosterDuration { get { return FinalBoosterDuration; } }
    public float ViewBoosterMult { get { return FinalBoosterMult; } }

    /// <summary>
    /// 플레이어가 조각배 탑승 중에 암초에 충돌할 경우
    /// </summary>
    IEnumerator ReefCrash()
    {
        DisableControls();
        animator.SetTrigger("ReefCrash");
        RuntimeManager.PlayOneShot(sound_SailboatBump);
        AbortBooster();
        driftActive = false;
        stunEffect.Play(true);
        stoneAttackEffect.Play(true);

        rBody.velocity = new Vector3(0f, rBody.velocity.y, 0f);
        rBody.AddForce(-transform.forward * reefCrashPower, ForceMode.Impulse);
        
        yield return new WaitForSeconds(reefCrashStifftime);


        SailboatQuit();
        //rBody.AddForce(Vector3.back * reefCrashPower, ForceMode.Impulse);
        //rBody.AddForce(Vector3.down * reefCrashPower, ForceMode.Impulse);
        yield return new WaitForSeconds(reefCrashbindTime);


        EnableControls();
    }

    float reefCrashStifftime = 0.5f;
    float reefCrashbindTime = 3.0f;
    float reefCrashPower = 15.0f;
    float boatGroundingTimer = 0f;

    /// <summary>
    /// 충돌감지
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Reef"))
        {
            //암초충돌감지
            if (previousVelocity.magnitude - rBody.velocity.magnitude > 10)
            {
                StartCoroutine(ReefCrash());
            }
        }

        if(((1 << collision.collider.gameObject.layer) & groundIgnore) == 0)
        {
            boatGroundingTimer = sailboatAutoOffTime;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.collider.gameObject.layer) & groundIgnore) == 0 && grounding)
        {
            if (CurrentMovement.GetType() == typeof(Movement_Sailboat))
            {
                boatGroundingTimer -= Time.deltaTime;
                if(boatGroundingTimer < 0)
                {
                    CurrentMovement = new Movement_Ground();
                }
            }
        }
    }

    public void JumpingFromObj()
    {
        JumpObject jumpObject = FindObjectOfType<JumpObject>();
        if (jumpObject != null)
        {
            float jumpingForce = jumpObject.jumpForce;
            rBody.AddForce(Vector3.up * jumpingForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("JumpObject를 찾을 수 없습니다!");
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.magenta;
        if (bottomColider != null)
        {
            if (groundNormal != Vector3.zero)
            {
                DrawArrow.ForGizmo(transform.position + bottomColider.center, -groundNormal * (groundCastDistance + bottomColider.radius));
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position + bottomColider.center, 0.1f);
            }
        }
    }
#endif
}
