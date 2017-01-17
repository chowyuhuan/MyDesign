using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using BUFF;
using ACTOR;
using UnityEditor;

namespace SKILL_EDITOR
{
    public sealed class DCMetaNode : SkillNodeBase
    {
        private ObjectToStringField _bulletPrefab = new ObjectToStringField();
        private ObjectToStringField _hitEffect = new ObjectToStringField();
        public DCMetaNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.DC) { }
        bool _foldoutMotion = false;
        protected override void Draw()
        {
            DCMeta Meta = MetaData as DCMeta;
            BeginResizeHeight();
            BackgroundColor = Meta.Intention == Intent.Cure ? Color.green : Color.red;
            EditorGUIUtility.labelWidth = 56;
            Meta.Intention = (Intent)EditorGUILayout.EnumPopup(new GUIContent("意图", "攻击 or 治疗"), Meta.Intention, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            Meta.CampEx = Meta.Intention == Intent.Damage ? Camp.Enemy : Camp.Comrade;
            //Meta.CampEx = (Camp)EditorGUILayout.EnumPopup(new GUIContent("阵营", "敌方 or 我方，相对自身"), Meta.CampEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            Meta.TargetEx = (Target)EditorGUILayout.EnumPopup(new GUIContent("目标", "攻击具体目标"), Meta.TargetEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            Meta.CarrierEx = (Carrier)EditorGUILayout.EnumPopup(new GUIContent("载体", "释放即命中 or 子弹技 or 武器挥砍"), Meta.CarrierEx, GUILayout.MaxWidth(SkillEditor.Width_Enum)); // 切换Mode的时候，考虑要不要清掉MotionMeta数据
            AddLine(3);
            if (Meta.CarrierEx == Carrier.Bullet)
            {
                Meta.TerminateModeEx = TerminateMode.Destroy;
                Meta.CollideLimit = EditorGUILayout.DelayedIntField(new GUIContent("命中限制", "最多可以打中多少人"), Meta.CollideLimit, GUILayout.MaxWidth(SkillEditor.Width_Int));
                AddLine();
                _foldoutMotion = DrawMotion(Meta, _foldoutMotion);
            }
            else if (Meta.CarrierEx == Carrier.Weapon)
            {
                Meta.TerminateModeEx = TerminateMode.DeactiveWeapon;
                Meta.CollideLimit = EditorGUILayout.DelayedIntField(new GUIContent("命中限制", "最多可以打中多少人"), Meta.CollideLimit, GUILayout.MaxWidth(SkillEditor.Width_Int));
                AddLine();
            }

            if (Meta.CarrierEx == Carrier.Bullet && 
                Meta.Motion.ForceHorizontal && 
                (Meta.Motion.Track == Navigation.SingleLine || Meta.Motion.Track == Navigation.MultipleLine_Rect || Meta.Motion.Track == Navigation.MultipleLine_Sect))
            {

            }
            else
            {
                Meta.Motion.LifeCycle = EditorGUILayout.DelayedFloatField(new GUIContent("持续时间", "时间（s）过后销毁"), Meta.Motion.LifeCycle, GUILayout.MaxWidth(SkillEditor.Width_Float));
                AddLine();
            }
            Meta.EmitSound = EditorGUILayout.DelayedTextField("发射音效", Meta.EmitSound);
            Meta.HitSound = EditorGUILayout.DelayedTextField("命中音效", Meta.HitSound);
            Meta.HitEffect = _hitEffect.ObjectField();
            AddLine(3);

            Meta.HitMotion = (HitMotion)EditorGUILayout.EnumPopup(new GUIContent("击中效果", "NatureBump：自然撞击击飞（方向由撞击点决定）\nManualBump：自定义撞击击飞（手动设置X、Y方向力）\nRetreat：击退\nTransfer：传送"), Meta.HitMotion, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            AddLine();
            switch (Meta.HitMotion)
            {
                case HitMotion.NatureBump:
                    Meta.Force1 = EditorGUILayout.DelayedFloatField(new GUIContent("力", "命中时子弹对目标的作用力"), Meta.Force1, GUILayout.MaxWidth(SkillEditor.Width_Float));
                    AddLine();
                    break;
                case HitMotion.ManualBump:
                    Meta.Force1 = EditorGUILayout.DelayedFloatField(new GUIContent("X力", "命中时子弹对目标的X方向作用力"), Meta.Force1, GUILayout.MaxWidth(SkillEditor.Width_Float));
                    Meta.Force2 = EditorGUILayout.DelayedFloatField(new GUIContent("Y力", "命中时子弹对目标的Y方向作用力"), Meta.Force2, GUILayout.MaxWidth(SkillEditor.Width_Float));
                    AddLine(2);
                    break;
                case HitMotion.Retreat:
                    Meta.Force1 = EditorGUILayout.DelayedFloatField(new GUIContent("力", "命中时子弹对目标的作用力"), Meta.Force1, GUILayout.MaxWidth(SkillEditor.Width_Float));
                    Meta.Force2 = EditorGUILayout.DelayedFloatField(new GUIContent("距离", "击退距离"), Meta.Force2, GUILayout.MaxWidth(SkillEditor.Width_Float));
                    AddLine(2);
                    break;
                case HitMotion.Transfer:
                    Meta.Force1 = EditorGUILayout.DelayedFloatField(new GUIContent("力", "命中时子弹对目标的作用力"), Meta.Force1, GUILayout.MaxWidth(SkillEditor.Width_Float));
                    Meta.Force2 = EditorGUILayout.DelayedFloatField(new GUIContent("位置", "相对于施法者，传送到的位置"), Meta.Force2, GUILayout.MaxWidth(SkillEditor.Width_Float));
                    AddLine(2);
                    break;
                case HitMotion.None:
                    break;
            }

            EditorGUILayout.BeginHorizontal();
            //if (GUILayout.Button("+ 触发器", SkillEditorUtility.LeftButton))
            //{
            //    if (Meta.Triggers == null || Meta.Triggers.Length <= 0)
            //    {
            //        CreateChild(Node.Trigger);
            //    }
            //}
            if (GUILayout.Button("-", SkillEditorUtility.MidButton))
            {
                TryToDestroy();
            }
            //if (GUILayout.Button("Buff +", SkillEditorUtility.RightButton))
            //{
            //    CreateChild(Node.Buff);
            //}
            if (GUILayout.Button("行为 +", SkillEditorUtility.RightButton))
            {
                BehaviorGenerateCubeMeta generateCubeMeta = new BehaviorGenerateCubeMeta();
                Meta.Behaviors.Add(generateCubeMeta);
                CreateChild(Node.TriggerBehavior, new ListWrapper<TriggerBehaviorMeta>(Meta.Behaviors, generateCubeMeta), false);
            }
            EditorGUILayout.EndHorizontal();
            AddLine();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ 攻击", SkillEditorUtility.LeftButton))
            {
                CreateChild(Node.VolAtk);
            }
            if (GUILayout.Button("属性 +", SkillEditorUtility.RightButton))
            {
                CreateChild(Node.VolField);
            }
            EditorGUILayout.EndHorizontal();
            AddLine();
            EndResizeHeight();
        }
        protected override SkillNodeBase CreateChildImp(Node idx, object data, bool archive)
        {
            DCMeta Meta = MetaData as DCMeta;
            if (idx == Node.VolAtk)
            {
                return CreateChildWithData<ATKVolumeNode, Volume.ATK>("数值:攻击", new Vector2(100, 130), ref Meta.VolumeEx.Atks, data, archive);
            }
            else if (idx == Node.VolField)
            {
                return CreateChildWithData<FieldVolumeNode, Volume.Field>("数值:属性", new Vector2(100, 130), ref Meta.VolumeEx.Fields, data, archive);
            }
            else if (idx == Node.TriggerBehavior)
            {
                return CreateChildWithData<TriggerBehaviorNode, WrapperMeta>("行为", new Vector2(150, 95), data, archive);
            }
            return null;
        }
        protected override void RemoveChildImp(SkillNodeBase node)
        {
            DCMeta Meta = MetaData as DCMeta;
            if (node.Tag == Node.VolAtk)
            {
                RemoveOneData<Volume.ATK>(ref Meta.VolumeEx.Atks, ref node.MetaData);
            }
            else if (node.Tag == Node.VolField)
            {
                RemoveOneData<Volume.Field>(ref Meta.VolumeEx.Fields, ref node.MetaData);
            }
            else if (node.Tag == Node.TriggerBehavior)
            {
                WrapperMeta Wrapper = node.MetaData as WrapperMeta;
                Meta.Behaviors.Remove((TriggerBehaviorMeta)Wrapper.Meta);
            }
        }
        public override void OnCreated()
        {
            DCMeta Meta = MetaData as DCMeta;
            SkillDetailEditor.BuildVolumeNode(ref Meta.VolumeEx, this);

            for (int i = 0; i < Meta.Behaviors.Count; i++)
            {
                CreateChild(Node.TriggerBehavior, new ListWrapper<TriggerBehaviorMeta>(Meta.Behaviors, Meta.Behaviors[i]), false);
            }
        }
        protected override void Init()
        {
            DCMeta Meta = MetaData as DCMeta;
            string path = "Assets/Resources/" + AssetManage.AM_PathHelper.GetActorEffectFullPathByName(Meta.Motion.PrefabName) + ".prefab";
            _bulletPrefab.Init(new GUIContent("Prefab"), path, typeof(GameObject), true);
            path = "Assets/Resources/" + AssetManage.AM_PathHelper.GetActorEffectFullPathByName(Meta.HitEffect) + ".prefab";
            _hitEffect.Init(new GUIContent("命中特效"), path, typeof(GameObject), false);
        }
        public bool DrawMotion(DCMeta meta, bool foldout)
        {
            foldout = EditorGUILayout.Foldout(foldout, "子弹");
            AddLine();
            if (foldout)
            {
                meta.Motion.PrefabName = _bulletPrefab.ObjectField();
                meta.Motion.AnchorEx = (Anchor)EditorGUILayout.EnumPopup(new GUIContent("锚点", "确定起始位置时，偏移的基准\nCaster：施法者\nTarget：被攻击目标\nPosition：手动指定具体位置"), meta.Motion.AnchorEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
                meta.Motion.AnchorOffset = EditorGUILayout.Vector2Field(new GUIContent("锚点偏移", "确定起始位置时，相对于锚点的偏移"), meta.Motion.AnchorOffset, GUILayout.MaxWidth(SkillEditor.Width_Vector2));
                meta.Motion.Track = (Navigation)EditorGUILayout.EnumPopup(new GUIContent("轨迹", "运动轨迹\nSingleLine：单直线\nSingleCurve：单曲线\nMultipleLine_Rect：矩形多直线\nMultipleLine_Sect：扇形多直线\nMultipleCurve：多曲线\nPrefab：Prefab自己控制运动轨迹"), meta.Motion.Track, GUILayout.MaxWidth(SkillEditor.Width_Enum));
                AddLine(5); // 其中AnchorOffset占2行
                switch (meta.Motion.Track)
                {
                    case Navigation.SingleLine:
                        meta.Motion.MoveSpeed = EditorGUILayout.DelayedFloatField("移动速度", meta.Motion.MoveSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.RotateStyle = (RotationStyle)EditorGUILayout.EnumPopup(new GUIContent("自转方式", "无旋转 or 围绕中心自转 or 自动校准朝向"), meta.Motion.RotateStyle, GUILayout.MaxWidth(SkillEditor.Width_Enum));
                        meta.Motion.RotateSpeed = EditorGUILayout.DelayedFloatField("自转速度", meta.Motion.RotateSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.ForceHorizontal = EditorGUILayout.Toggle(new GUIContent("仅横向", "限制纵轴运动，仅水平运动"), meta.Motion.ForceHorizontal);
                        AddLine(4);
                        if (meta.Motion.ForceHorizontal)
                        {
                            float range = meta.Motion.LifeCycle * meta.Motion.MoveSpeed;
                            range = EditorGUILayout.DelayedFloatField("射程", range, GUILayout.MaxWidth(SkillEditor.Width_Float));
                            meta.Motion.LifeCycle = range / meta.Motion.MoveSpeed;
                            AddLine();
                        }
                        break;
                    case Navigation.MultipleLine_Rect:
                        meta.Motion.MoveSpeed = EditorGUILayout.DelayedFloatField("移动速度", meta.Motion.MoveSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.RotateStyle = (RotationStyle)EditorGUILayout.EnumPopup(new GUIContent("自转方式", "无旋转 or 围绕中心自转 or 自动校准朝向"), meta.Motion.RotateStyle, GUILayout.MaxWidth(SkillEditor.Width_Enum));
                        meta.Motion.RotateSpeed = EditorGUILayout.DelayedFloatField("自转速度", meta.Motion.RotateSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.ExtendValue = EditorGUILayout.DelayedFloatField(new GUIContent("区间", "上下浮动的空间"), meta.Motion.ExtendValue, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.ForceHorizontal = EditorGUILayout.Toggle(new GUIContent("仅横向", "限制纵轴运动，仅水平运动"), meta.Motion.ForceHorizontal);
                        AddLine(5);
                        if (meta.Motion.ForceHorizontal)
                        {
                            float range = meta.Motion.LifeCycle * meta.Motion.MoveSpeed;
                            range = EditorGUILayout.DelayedFloatField("射程", range, GUILayout.MaxWidth(SkillEditor.Width_Float));
                            meta.Motion.LifeCycle = range / meta.Motion.MoveSpeed;
                            AddLine();
                        }
                        break;
                    case Navigation.MultipleLine_Sect:
                        meta.Motion.MoveSpeed = EditorGUILayout.DelayedFloatField("移动速度", meta.Motion.MoveSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.RotateStyle = (RotationStyle)EditorGUILayout.EnumPopup(new GUIContent("自转方式", "无旋转 or 围绕中心自转 or 自动校准朝向"), meta.Motion.RotateStyle, GUILayout.MaxWidth(SkillEditor.Width_Enum));
                        meta.Motion.RotateSpeed = EditorGUILayout.DelayedFloatField("自转速度", meta.Motion.RotateSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.ExtendValue = EditorGUILayout.DelayedFloatField(new GUIContent("角度", "扇形角度"), meta.Motion.ExtendValue, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.ForceHorizontal = EditorGUILayout.Toggle(new GUIContent("仅横向", "限制纵轴运动，仅水平运动"), meta.Motion.ForceHorizontal);
                        AddLine(5);
                        if (meta.Motion.ForceHorizontal)
                        {
                            float range = meta.Motion.LifeCycle * meta.Motion.MoveSpeed;
                            range = EditorGUILayout.DelayedFloatField("射程", range, GUILayout.MaxWidth(SkillEditor.Width_Float));
                            meta.Motion.LifeCycle = range / meta.Motion.MoveSpeed;
                            AddLine();
                        }
                        break;
                    case Navigation.SingleCurve:
                        meta.Motion.MoveSpeed = EditorGUILayout.DelayedFloatField("移动速度", meta.Motion.MoveSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.RotateStyle = (RotationStyle)EditorGUILayout.EnumPopup(new GUIContent("自转方式", "无旋转 or 围绕中心自转 or 自动校准朝向"), meta.Motion.RotateStyle, GUILayout.MaxWidth(SkillEditor.Width_Enum));
                        meta.Motion.RotateSpeed = EditorGUILayout.DelayedFloatField("自转速度", meta.Motion.RotateSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.Range = EditorGUILayout.DelayedFloatField(new GUIContent("高度", "曲线拱高"), meta.Motion.Range, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        AddLine(4);
                        break;
                    case Navigation.MultipleCurve:
                        meta.Motion.MoveSpeed = EditorGUILayout.DelayedFloatField("移动速度", meta.Motion.MoveSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.RotateStyle = (RotationStyle)EditorGUILayout.EnumPopup(new GUIContent("自转方式", "无旋转 or 围绕中心自转 or 自动校准朝向"), meta.Motion.RotateStyle, GUILayout.MaxWidth(SkillEditor.Width_Enum));
                        meta.Motion.RotateSpeed = EditorGUILayout.DelayedFloatField("自转速度", meta.Motion.RotateSpeed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.Range = EditorGUILayout.DelayedFloatField(new GUIContent("高度", "曲线拱高"), meta.Motion.Range, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        meta.Motion.ExtendValue = EditorGUILayout.DelayedFloatField(new GUIContent("区间", "水平浮动空间"), meta.Motion.ExtendValue, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        AddLine(5);
                        break;
                    case Navigation.Prefab:
                        meta.Motion.ImpactWayEx = (ImpactWay)EditorGUILayout.EnumPopup(new GUIContent("生效方式", "Once：一次性伤害\nInterval：间隔性伤害\n间隔时间由子弹的间隔属性决定"), meta.Motion.ImpactWayEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
                        AddLine();
                        break;
                }

                if (meta.Motion.Track == Navigation.Prefab)
                {
                    if (meta.Motion.ImpactWayEx == ImpactWay.Interval)
                    {
                        meta.Motion.Interval = EditorGUILayout.DelayedFloatField(new GUIContent("间隔", "每隔多长时间生效一次"), meta.Motion.Interval, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        AddLine();
                    }
                }
                else
                {
                    meta.Motion.EntityNum = EditorGUILayout.DelayedIntField(new GUIContent("数量", "子弹数量"), meta.Motion.EntityNum, GUILayout.MaxWidth(SkillEditor.Width_Int));
                    AddLine();
                    if (meta.Motion.EntityNum > 1)
                    {
                        meta.Motion.Interval = EditorGUILayout.DelayedFloatField(new GUIContent("间隔", "发射子弹之间的间隔"), meta.Motion.Interval, GUILayout.MaxWidth(SkillEditor.Width_Float));
                        AddLine();
                    }
                }
            }
            return foldout;
        }
    }
}
