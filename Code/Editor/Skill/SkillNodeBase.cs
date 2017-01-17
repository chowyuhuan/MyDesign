using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SKILL;
using ACTOR;
using UnityEditor;

namespace SKILL_EDITOR
{
    public abstract class SkillNodeBase
    {
        public static bool NeedRepaint = false;
        protected static int GWinID = 0;

        readonly public int Layer = -1;
        readonly public int WinID = -1;
        readonly public Node Tag = Node.DC; // 扩展字段，用于区分Node，并不是所有地方都会使用
        readonly public string Title = "技能";
        readonly public SkillNodeBase ParentNode = null;
        // 白
        // 黑
        // （122，255,255）蓝绿
        // (255,160,0) 橙黄
        // 红
        // 蓝
        // 绿
        // 黄
        // 深紫（127,0,127）
        public Color BackgroundColor;

        public Vector2 Size = Vector2.zero;
        public Rect Rect = new Rect(0, 0, 100, 100);
        public bool Visual = true;

        public MetaBase MetaData;

        protected List<SkillNodeBase> SubNodes = new List<SkillNodeBase>();
        protected bool Inited = false;

        private Rect _preRect;

        public SkillNodeBase(SkillNodeBase parent, int layer, string title, Vector2 size, Node tag, Color color)
        {
            ParentNode = parent;
            Layer = layer;
            WinID = GWinID++;
            Title = title;
            Size = size;
            Tag = tag;
            BackgroundColor = color;
        }

        public SkillNodeBase(SkillNodeBase parent, int layer, string title, Vector2 size, Node tag)
        {
            ParentNode = parent;
            Layer = layer;
            WinID = GWinID++;
            Title = title;
            Size = size;
            Tag = tag;
            BackgroundColor = GUI.backgroundColor;
        }

        public void Show()
        {
            Visual = true;
        }

        public void Hide()
        {
            Visual = false;
        }

        public bool IsShow()
        {
            return Visual;
        }

        public Rect DrawNode()
        {
            if (!Inited)
            {
                Init();
                _preRect = Rect;
                Inited = true;
            }

            GUI.backgroundColor = BackgroundColor;
            _preRect = Rect;
            Rect = GUI.Window(WinID, Rect, Draw, Title);
            Vector2 move = new Vector2(Rect.xMin - _preRect.xMin, Rect.yMin - _preRect.yMin);
            _preRect = Rect;
            for (int i = 0; i < SubNodes.Count; ++i)
            {
                SkillNodeBase child = SubNodes[i];
                if (Event.current.control)
                {
                    child.Move(move, true);
                }
                if (child.IsShow())
                {
                    Rect subRect = child.DrawNode();
                    SkillDetailEditor.DrawNodeCurve(Rect, subRect, child.BackgroundColor);
                }
            }
            return Rect;
        }

        public SkillNodeBase GetNodeWithPos(Vector2 pos)
        {
            if (Rect.Contains(pos))
            {
                return this;
            }
            for (int i = 0; i < SubNodes.Count; ++i)
            {
                SkillNodeBase child = SubNodes[i];
                SkillNodeBase subChild = child.GetNodeWithPos(pos);
                if (subChild != null)
                {
                    return subChild;
                }
            }
            return null;
        }

        public void Move(Vector2 delta, bool recursive)
        {
            Rect.x += delta.x;
            Rect.y += delta.y;
            if (recursive)
            {
                for (int i = 0; i < SubNodes.Count; ++i)
                {
                    SubNodes[i].Move(delta, recursive);
                }
            }
        }

        void Draw(int id)
        {
            Draw();
            Input();
            //DragWindow(new Rect(Rect.position, new Vector2(Rect.width, 15)));
            GUI.DragWindow();

        }

        protected void SetDeltaHeight(float value)
        {
            Size.y += value;
            Rect.height += value;
        }

        public SkillNodeBase CreateChild(Node idx, object data = null, bool archive = true)
        {
            SkillNodeBase node = CreateChildImp(idx, data, archive);
            if (node != null)
            {
                SubNodes.Add(node);
                AddGNode(node);
                node.OnCreated();
            }
            return node;
        }

        public void TryToDestroy()
        {
            if (EditorUtility.DisplayDialog("警告", "确定要删除此节点吗？", "确定", "取消"))
            {
                Destroy();
            }
        }

        public void Destroy()
        {
            while (SubNodes.Count > 0)
            {
                SubNodes[0].Destroy();
            }
            if (ParentNode != null)
            {
                ParentNode.RemoveChild(this);
            }
        }

        public void RemoveChild(SkillNodeBase node)
        {
            SubNodes.Remove(node);
            RemoveChildImp(node);
            RemoveGNode(node);
        }

        public SkillNodeBase GetRoot()
        {
            SkillNodeBase root = this;
            while (root.ParentNode != null)
            {
                root = root.ParentNode;
            }
            return root;
        }

        void Input()
        {
            Event e = Event.current;
            if (e.type == UnityEngine.EventType.MouseDown && e.button == 1)
            {
                SkillClipboard.Copy(MetaData);
            }
        }


        public T CreateChildWithData<T>(string title, Vector2 size, bool numPreifx = true) where T : SkillNodeBase
        {
            int offset = GetLayerNodeNum(Layer + 1);
            int count = SubNodes.Count;
            T node = (T)Activator.CreateInstance(typeof(T), this, Layer + 1, numPreifx ? count.ToString() + "-" + title : title, size);
            float x = count > 0 ? SubNodes[count - 1].Rect.xMax + 5 : Rect.xMin;
            float y = count > 0 ? SubNodes[count - 1].Rect.yMin : Rect.yMax + 15;
            node.Rect = new Rect(x, y, node.Size.x, node.Size.y);
            return node;
        }

        public NodeT CreateChildWithData<NodeT, MetaT>(string title, Vector2 size, ref MetaT[] metaArray, object data, bool archive, bool numPreifx = true) 
            where NodeT : SkillNodeBase
            where MetaT : MetaBase, new()
        {
            int offset = GetLayerNodeNum(Layer + 1);
            int count = SubNodes.Count;
            NodeT node = (NodeT)Activator.CreateInstance(typeof(NodeT), this, Layer + 1, numPreifx ? count.ToString() + "-" + title : title, size);
            float x = count > 0 ? SubNodes[count - 1].Rect.xMax + 5 : Rect.xMin;
            float y = count > 0 ? SubNodes[count - 1].Rect.yMin : Rect.yMax + 15;
            node.Rect = new Rect(x, y, node.Size.x, node.Size.y);

            MetaT meta = null;
            if (data == null)
            {
                data = SkillClipboard.Paste<MetaT>();
                if (data == null)
                {
                    data = new MetaT();
                }
            }

            meta = data as MetaT;

            if (archive)
            {
                metaArray = EditorArrayHelper.AddOne<MetaT>(ref metaArray, ref meta);
            }

            node.MetaData = meta;
            return node;
        }

        public NodeT CreateChildWithData<NodeT, MetaT>(string title, Vector2 size, object data, bool archive, bool numPreifx = true) 
            where NodeT : SkillNodeBase
            where MetaT : MetaBase, new()
        {
            int offset = GetLayerNodeNum(Layer + 1);
            int count = SubNodes.Count;
            NodeT node = (NodeT)Activator.CreateInstance(typeof(NodeT), this, Layer + 1, numPreifx ? count.ToString() + "-" + title : title, size);
            float x = count > 0 ? SubNodes[count - 1].Rect.xMax + 5 : Rect.xMin;
            float y = count > 0 ? SubNodes[count - 1].Rect.yMin : Rect.yMax + 15;
            node.Rect = new Rect(x, y, node.Size.x, node.Size.y);

            MetaT meta = null;
            if (data == null)
            {
                data = SkillClipboard.Paste<MetaT>();
                if (data == null)
                {
                    data = new MetaT();
                }
            }

            meta = data as MetaT;

            node.MetaData = meta;
            return node;
        }

        public static void RemoveOneData<T>(ref T[] array, ref MetaBase element)
        {
            if (array.Length - 1 == 0)
            {
                array = null;
                return;
            }
            T[] temp = new T[array.Length - 1];
            int j = 0;
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i].Equals(element))
                {
                    continue;
                }
                temp[j++] = array[i];
            }
            array = temp;
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="metaArray">要被归档到的数组</param>
        /// <param name="data">已经存在的数据</param>
        /// <param name="archive">是否要被归档</param>
        /// <returns></returns>
        public T CreateData<T>(ref T[] metaArray, object data, bool archive) where T : MetaBase, new()
        {
            T meta = null;
            if(data == null)
            {
                data = SkillClipboard.Paste<T>();
                if(data == null)
                {
                    data = new T();
                }
            }

            meta = data as T;

            if(archive)
            {
                metaArray = EditorArrayHelper.AddOne<T>(ref metaArray, ref meta);
            }
            return meta;
        }

        ///// <summary>
        ///// data不为null，意味着数据已经存在，且已在metaArray中
        ///// </summary>
        //public T CreateData<T>(ref T[] metaArray, object data) where T : MetaBase, new()
        //{
        //    if (data == null)
        //    {
        //        T meta = new T();
        //        metaArray = EditorArrayHelper.AddOne<T>(ref metaArray, ref meta);
        //        return meta;
        //    }
        //    else
        //    {
        //        return data as T;
        //    }
        //}

        #region 动态高度
        float _height = 0;
        public void BeginResizeHeight()
        {
            _height = 26; // 边框
        }
        public void AddHeight(float value)
        {
            _height += value;
        }
        public void AddLine(int count = 1)
        {
            _height += count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
        public void EndResizeHeight()
        {
            Size.y = _height;
            Rect.height = _height;
        }
        #endregion


        #region 全局节点
        public static Dictionary<int, List<SkillNodeBase>> GNodes = new Dictionary<int, List<SkillNodeBase>>();

        public static void AddGNode(SkillNodeBase node)
        {
            List<SkillNodeBase> nodes;
            if (!GNodes.TryGetValue(node.Layer, out nodes))
            {
                nodes = new List<SkillNodeBase>();
                GNodes.Add(node.Layer, nodes);
            }
            nodes.Add(node);
        }

        public static void RemoveGNode(SkillNodeBase node)
        {
            List<SkillNodeBase> nodes;
            if (GNodes.TryGetValue(node.Layer, out nodes))
            {
                nodes.Remove(node);
            }
        }
        public static int GetLayerNodeNum(int layer)
        {
            List<SkillNodeBase> nodes;
            if (GNodes.TryGetValue(layer, out nodes))
            {
                return nodes.Count;
            }
            return 0;
        }
        public static void ClearGNodes()
        {
            GNodes.Clear();
        }
        #endregion

        protected virtual SkillNodeBase CreateChildImp(Node idx, object data, bool archive) { return null; }
        protected virtual void RemoveChildImp(SkillNodeBase node) { }
        protected virtual void Init() { }
        protected abstract void Draw();
        public virtual void OnCreated() { }
        public virtual void ShowPopup(Vector2 pos) { }


    }

}