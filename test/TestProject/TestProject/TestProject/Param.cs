using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    /// <summary>
    /// 積極單例模式
    /// </summary>
    public class SingleObject
    {
        //創建 SingleObject 的一個對象
        private static readonly SingleObject instance = new SingleObject();
        private int a;
        private string b;

        //讓構造函數為 private，這樣該類就不會被實例化
        private SingleObject() 
        {
            a = 0;
            b = "0";
        }

        //獲取唯一可用的對象
        public static SingleObject GetInstance()
        {
            return instance;
        }
        public static int GetInstanceInt()
        {
            instance.a += 5;
            return instance.a;
        }
        public static string GetInstanceStr()
        {
            instance.b += "WA";
            return instance.b;
        }
    }
    //----------------------------------
    /// <summary>
    /// 正方形箱子
    /// </summary>
    class SquareBox
    {
        public SquareBox()
        {
            Console.WriteLine("正方形箱子");
        }
    }
    /// <summary>
    /// 三角形箱子
    /// </summary>
    class TriangleBox
    {
        public TriangleBox()
        {
            Console.WriteLine("三角形箱子");
        }
    }
    /// <summary>
    /// 圓形箱子
    /// </summary>
    class CircleBox
    {
        public CircleBox()
        {
            Console.WriteLine("圓形箱子");
        }
    }
    /// <summary>
    /// 箱子工廠
    /// </summary>
    class BoxFactory
    {
        public BoxFactory()
        {
            Console.WriteLine("這是一間箱子工廠");
        }
        /// <summary>
        /// 通用BoxMaker
        /// </summary>
        class GenericBoxMaker
        {
            public GenericBoxMaker()
            {
                Console.WriteLine("箱子製造機建置完成");
            }
            public object GetBox<T>() where T : new()
            {
                Console.WriteLine("產生合適的箱子");
                return new T();
            }
        }
        public void Start()
        {
            Console.WriteLine("工廠開始運作");
            GenericBoxMaker maker = new GenericBoxMaker();
            maker.GetBox<SquareBox>();
            maker.GetBox<TriangleBox>();
            maker.GetBox<CircleBox>();
        }
    }
    //-----------------------------------
    


    //================================
    class Param
    {
    }
}
