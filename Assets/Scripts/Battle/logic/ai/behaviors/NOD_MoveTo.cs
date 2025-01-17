﻿#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Nod_Move.cs
 Author:      Zeng Zhiwei
 Time:        2020/3/4 20:01:27
=====================================================
*/
#endregion

using UnityEngine;

namespace TsiU
{
    public class NOD_MoveTo : TBTActionLeaf
    {
        protected override void onEnter(TBTWorkingData wData)
        {
            //BattleLog.Log("【NOD_MoveTo】onEnter");
            BattleBehaviorWorkingData behaviorData = wData as BattleBehaviorWorkingData;
            BattleUnit source = behaviorData.owner;
            source.SetState(HeroState.MOVE);
        }

        protected override void onExit(TBTWorkingData wData, int runningStatus)
        {
            //BattleLog.Log("【NOD_MoveTo】onExit");
        }

        protected override int onExecute(TBTWorkingData wData)
        {
            BattleBehaviorWorkingData behaviorData = wData as BattleBehaviorWorkingData;
            float deltaTime = behaviorData.deltaTime;
            BattleUnit source = behaviorData.owner;
            AIBehaviorRequest request = behaviorData.request;
            Unit target = request.target;

            Vector2 ownerPos = source.Get2DPosition();
            Vector2 targetPos = target.Get2DPosition();

            float attackRange = source.GetAttackRange();
            float distance = (targetPos - ownerPos).magnitude;

            if(distance <= attackRange)
                return TBTRunningStatus.FINISHED;

            float moveSpeed = source.GetMoveSpeed();
            float detalTime = behaviorData.deltaTime;
            float toForwardX = (targetPos.x - ownerPos.x) / distance;
            float toForwardZ = (targetPos.y - ownerPos.y) / distance;
            float newPosX = ownerPos.x + detalTime * moveSpeed * toForwardX;
            float newPosZ = ownerPos.y + detalTime * moveSpeed * toForwardZ;

            source.Set2DPosition(newPosX, newPosZ);
            //BattleLog.Log("【NOD_MoveTo】移动速度：{0} 当前位置：{1},{2} 目标位置：{3},{4} newPosX：{5} newPosZ：{6}}", 
            //    moveSpeed.ToString(), ownerPos.x.ToString(),ownerPos.y.ToString(), targetPos.x.ToString(),targetPos.y.ToString()
            //    ,newPosX.ToString(),newPosZ.ToString());

            GameMsg.instance.SendMessage(GameMsgDef.Hero_MoveTo, new HeorMoveEventArgs()
            {
                id = source.id,
                targetPos = new Vector3(newPosX, 0, newPosZ),
                forward = new Vector3(toForwardX,0, toForwardZ)
            });
            return TBTRunningStatus.EXECUTING;
        }
    }
}