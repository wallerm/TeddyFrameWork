﻿#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    HudActorManager.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\1 星期二 23:44:31
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public class HudActorManager : Singleton<HudActorManager>
{
    private const string path = "battle/component/HpBar";
    public Transform parentNode; 
    private RectTransform canvasTransform;
    private Camera uiCamera;
    private Dictionary<int, HudActor> m_actorMap;

    public void Init(RectTransform hudParent)
    {
        m_actorMap = new Dictionary<int, HudActor>();

        uiCamera = CameraManager.instance.uiCamera;
        canvasTransform = hudParent;
        parentNode = hudParent;

        GameMsg.instance.AddMessage(GameMsgDef.BattleEntity_HP_Updated, this, new EventHandler<EventArgs>(OnHeroHPUpdated));
    }

    ~HudActorManager()
    {
        GameMsg.instance.RemoveMessage(GameMsgDef.BattleEntity_HP_Updated);
    }

    public void OnHeroActorCreated(object sender, EventArgs args)
    {
        BattleActorCreateEventArgs arg = args as BattleActorCreateEventArgs;
        HeroActor actor = arg.heroActor;
        bool isFriend = arg.isFriend;

        // 创建一个血条
        LoadModule.LoadUI(path,delegate(AssetRequest data) {
            GameObject asset = data.asset as GameObject;

            GameObject go = Object.Instantiate(asset);
            go.transform.SetParent(parentNode);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            GameObject heroGO = actor.gameObject;
            HudController hudController = go.GetComponent<HudController>();
            Transform targetTransform = heroGO.transform.Find("Dummy OverHead");
            hudController.Init(canvasTransform, targetTransform, uiCamera, 0, isFriend);
            HudActor hudActor = new HudActor(go, hudController);
            float hpPercent = actor.battleEntity.GetHPPercent();
            hudActor.SetValue(hpPercent);

            m_actorMap.Add(actor.id, hudActor);
        });
    }

    public void OnHeroHPUpdated(object sender, EventArgs args)
    {
        HeorHPUpdateEventArgs arg = args as HeorHPUpdateEventArgs;
        int actorID = arg.id;

        HudActor actor;
        m_actorMap.TryGetValue(actorID, out actor);
        if(actor!=null)
        {
            float percent = (float)arg.curHp / arg.maxHp;

            actor.SetValue(percent);
        }
    }
}