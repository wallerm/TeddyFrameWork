﻿// 共享行为树
//http://www.aisharing.com/archives/750
using TsiU;

public class BehaviorTreeFactory : Singleton<BehaviorTreeFactory>
{
	private static TBTActionPrioritizedSelector m_DecisionTree;
	private static TBTActionPrioritizedSelector m_BehaviorTreeNode;

	public static TBTActionPrioritizedSelector GetDecisionTree()
	{
		if(m_DecisionTree == null)
		{
			m_DecisionTree = new TBTActionPrioritizedSelector();
			m_DecisionTree.SetPrecondition(new CON_CanUpdateRequest());
			m_DecisionTree.AddChild(new NOD_UpdateRequest());
		}
		return m_DecisionTree;
	}

	public static TBTActionPrioritizedSelector GetBehaviorTree()
	{
		if(m_BehaviorTreeNode == null)
		{
			// 父节点
			m_BehaviorTreeNode = new TBTActionPrioritizedSelector();

			// 设置idle节点
			var setUnitIdleNode = new NOD_SetUnitIdle();
			// 转向节点
			var turnToSelectorNode = new TBTActionPrioritizedSelector();
			turnToSelectorNode.SetPrecondition(new CON_IsAngleNeedTurnTo());
			turnToSelectorNode.AddChild(new NOD_TurnTo());//todo can turn to
			turnToSelectorNode.AddChild(setUnitIdleNode);
			// 追逐节点
			var moveToSelectorNode = new TBTActionPrioritizedSelector();
			moveToSelectorNode.SetPrecondition(new CON_IsInRange());
			moveToSelectorNode.AddChild(new NOD_MoveTo());//todo can Move to
			moveToSelectorNode.AddChild(setUnitIdleNode);

			// ①追逐
			TBTAction chaseSelectorNode = new TBTActionPrioritizedSelector().SetPrecondition(new CON_IsChaseRequest());
			chaseSelectorNode.AddChild(turnToSelectorNode);
			chaseSelectorNode.AddChild(moveToSelectorNode);

			// ②自动战斗节点构建
			// 转向和追逐并行
			var abilityTurnMoveToParallelNode = new TBTActionParallel();
			// 技能转向节点
			TBTPreconditionAND abilityTurnCondition = new TBTPreconditionAND(new CON_IsAbilityNeedTurnTo(), new CON_IsAngleNeedTurnTo());
			abilityTurnMoveToParallelNode.AddChild(new NOD_TurnTo().SetPrecondition(abilityTurnCondition));
			// 技能追逐节点
			TBTPreconditionNOT abilityMoveCondition = new TBTPreconditionNOT(new CON_IsInAbilityRange());
			abilityTurnMoveToParallelNode.AddChild(new NOD_MoveTo().SetPrecondition(abilityMoveCondition));
			abilityTurnMoveToParallelNode.SetEvaluationRelationship(TBTActionParallel.ECHILDREN_RELATIONSHIP.OR);

			// 技能施法节点
			var castAbilitySelectorNode = new TBTActionPrioritizedSelector();
			castAbilitySelectorNode.SetPrecondition(new CON_CanCastAbility());
			castAbilitySelectorNode.AddChild(new NOD_CastAbility());
			castAbilitySelectorNode.AddChild(setUnitIdleNode);

			var autoCastAbilityNode = new TBTActionPrioritizedSelector().SetPrecondition(new CON_IsAutoCastAbilityRequest());
			
			// 转向、移动、施法
			autoCastAbilityNode.AddChild(abilityTurnMoveToParallelNode);
			autoCastAbilityNode.AddChild(castAbilitySelectorNode);


			/// 开始构造树
			m_BehaviorTreeNode.AddChild(chaseSelectorNode);
			m_BehaviorTreeNode.AddChild(autoCastAbilityNode);
		}

		return m_BehaviorTreeNode;
	}
}
