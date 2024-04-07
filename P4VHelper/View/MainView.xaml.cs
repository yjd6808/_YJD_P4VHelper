// jdyun 24/04/06(토)

using System.Diagnostics;
using Gma.DataStructures.StringSearch;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace P4VHelper.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        Random r = new();

        void CreateNewString(StringBuilder b)
        {
            for (int j = 0; j < r.Next(3, 40); ++j)
            {
                // b.Append((char)r.Next(0xAC00, 0xD000));
                b.Append((char)r.Next('a', 'a' + 4));
            }
        }

        public MainView()
        {
            InitializeComponent();

            StringBuilder b = new();
            SortedSet<string> s = new();
            DateTime g = DateTime.Now;

            
            for (int i = 0; i < 10000; ++i)
            {
                CreateNewString(b);
                s.Add(b.ToString());
                b.Length = 0;
            }
            Debug.WriteLine((DateTime.Now - g).TotalMilliseconds);

            for (int kk = 0; kk < 20; ++kk)
            {
                var trie = new UkkonenTrie<int>(3);
                g = DateTime.Now;
                int gdrg = 0;
                foreach (var zz in s)
                {
                    trie.Add(zz, gdrg++);
                }
                Debug.WriteLine((DateTime.Now - g).TotalMilliseconds);
            }
            

            // ab가 포함된 문자열 찾음
            // 하나씩 다음 문자열 뽑으면서 완전 동일한 위치에 ab가 안나오는 구간까지 체크
            // 이후 해당 iterator부터 마지막 iterator까지 이진탐색해서 다시 이전 ab가 검출된 마지막 인덱스이후로 문자열을 비교해서 ab가 포함된 문자열을 찾는다.
            // 반복
            string k = "abcca";
            g = DateTime.Now;
            foreach (var kk in s)
            {
                kk.Contains(k);
            }

            
            //var trie = new SuffixTrie<int>(3);


            Debug.WriteLine((DateTime.Now - g).TotalMilliseconds);

            

            //int max = 100;
            //int l = 0;
            //foreach (string k in s)
            //{
            //    var ggg = trie.Retrieve(k.Substring(5, 100));
            //    int cnt = ggg.Count();
            //    ++l;
            //    if (l> max)
            //        break;
            //}
            Debug.WriteLine((DateTime.Now - g).TotalMilliseconds);
        }
    }
}