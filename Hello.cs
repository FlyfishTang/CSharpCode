public class Program
{
    public static void main()
    {
       var tree = InitTree();
       XmlHelper.CreateDocument(tree,x=>x.StartNode);
    }
    private Tree InitTree()
    {
        Tree tree = new Tree();
        for (int i = 0; i < 15; i++)
        {
            Random random = new Random();
            TreeNode treeNode = new TreeNode();
            if (tree.StartNode == null)
            {
                tree.StartNode = treeNode;
                treeNode.Name = "开始节点";
                treeNode.Index = 0;
                tree.StartNode = treeNode;
            }
            else
            {
                treeNode.Name = "子节点" + i;
                treeNode.Index = i;
                if (i <= 3)
                {
                    tree.StartNode.Children.Add(treeNode);
                }
                if (i == 4)
                {
                    tree.StartNode.Children.Find(x => x.Index == 1).Children.Add(treeNode);
                }
                if (i == 5 || i == 6)
                {
                    tree.StartNode.Children.Find(x => x.Index == 1).Children.Find(x => x.Index == 4).Children.Add(treeNode);
                }
                if (i == 7)
                {
                    tree.StartNode.Children.Find(x => x.Index == 2).Children.Add(treeNode);
                }
                if (i > 7)
                {
                    int num = random.Next(0, tree.ChildCount);
                    tree.AllTreeNodes.Find(x => x.Index == num).Children.Add(treeNode);
                }
            }
        }
        return tree;
    }
}
    /// <summary>
    /// 树
    /// </summary>
    public class Tree
    {
        /// <summary>
        /// 树构造函数
        /// </summary>
        public Tree()
        {
            //StartNode = new TreeNode();
        }

        /// <summary>
        /// 带节点的树初始化函数
        /// </summary>
        /// <param name="treeNode"></param>
        public Tree(TreeNode treeNode)
        {

            StartNode = treeNode;

        }
        public int ChildCount
        {
            get
            {
                return StartNode.GetChildrenCount();
            }
        }
        public TreeNode StartNode { get; set; }

        /// <summary>
        /// 所有节点集合
        /// </summary>
        public List<TreeNode> AllTreeNodes
        {
            get
            {
                List<TreeNode> rst = new List<TreeNode>();
                rst.Add(StartNode);
                rst.AddRange(AllChildren);
                return rst;
            }
        }

        /// <summary>
        /// 所有孩子节点集合
        /// </summary>
        public List<TreeNode> AllChildren
        {
            get
            {
                List<TreeNode> rst = new List<TreeNode>();
                rst.AddRange(StartNode.GetChildren());
                return rst;
            }
        }
    }
    /// <summary>
    /// 节点数据结构
    /// </summary>
    public class TreeNode
    {

        /// <summary>
        /// 树节点构造函数
        /// </summary>
        public TreeNode()
        {
            Children = new List<TreeNode>();
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeNode> Children { get; set; }

        /// <summary>
        /// 子节点个数
        /// </summary>
        /// <returns></returns>
        public int GetChildrenCount()
        {
            return GetChildren().Count;
        }

        /// <summary>
        /// 得到孩子节点
        /// </summary>
        /// <returns></returns>
        public List<TreeNode> GetChildren()
        {
            var treeNodes = new List<TreeNode>();
            treeNodes.AddRange(Children);
            treeNodes.AddRange(Children.SelectMany(x => x.GetChildren()));
            return treeNodes;
        }
    }
    public class XmlHelper
    {
        public static XDocument CreateDocument<T, TStart>(T tree, Func<T, TStart> func)
        {
           // string path = @"d:\1.xml";
            XElement xElement = GetXElementByTree(tree, func);
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xElement);
            return xdoc;
            // xdoc.Save(path);
        }

        public static XElement GetXElementByTree<T, TStart>(T t, Func<T, TStart> func)
        {
            XElement xElement = new XElement(t.GetType().Name);
            xElement.Add(GetXElement(func.Invoke(t)));
            return xElement;
        }

        public static XElement GetXElement<T>(T t)
        {
            XElement xElement = new XElement(t.GetType().Name);//TreeNode
            var properties = t.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType != typeof(List<T>))
                {
                    xElement.Add(new XElement(propertyInfo.Name, t.GetProperty(propertyInfo.Name)));
                }
                else
                {
                    var lst = t.GetProperty(propertyInfo.Name);
                    if (lst is List<T>)
                    {
                        (lst as List<T>).ToList().ForEach(x => xElement.Add(GetXElement(x)));
                    }
                }
            }
            return xElement;
        }
    }
