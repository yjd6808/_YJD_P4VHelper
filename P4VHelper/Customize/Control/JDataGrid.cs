// jdyun 24/05/07(화) - 전사 휴일
// 2가지 문제 때문에 구현함.
// 1. 하... 기존 데이터그리드는 이상하게 맨 밑으로 스크롤 내린상태에서 아이템을 추가하면
//    그 상태에서 Arrow Down 또는 Page Down을 꾹누르고 있으면 스크롤링 과정이 무시되고 맨 밑바닥으로 그냥 ScrollToEnd가 되버림
//    그래서 야매로 아이템이 추가되고 난 후 일정시간동안 방향키 입력을 차단하고자 함.
// 2. Page Down키는 스크롤 밑바닥에서 아이템이 추가되고 난 후 DataGrid로 포커스만 유지해주면 키가 올바르게 동작하는데
//    방향키는 포커스를 잡아줘도 인식을 못한다. 이때문에 기존 PageDown 동작방식을 따라서 처리하기로 함
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using P4VHelper.Extension;

namespace P4VHelper.Customize.Control
{
    public class JDataGridItemInfo
    {
        internal object Item { get; private set; }
        internal DependencyObject Container { get; set; }
        internal int Index { get; set; }

        public JDataGridItemInfo(object _item, DependencyObject _container = null, int _index = -1)
        {
            Item = _item;
            Container = _container;
            Index = _index;
        }

        public JDataGridItemInfo Refresh(ItemContainerGenerator _generator)
        {
            if (Container == null && Index < 0)
            {
                Container = _generator.ContainerFromItem(Item);
            }

            if (Index < 0 && Container != null)
            {
                Index = _generator.IndexFromContainer(Container);
            }

            if (Container == null && Index >= 0)
            {
                Container = _generator.ContainerFromIndex(Index);
            }

            return this;
        }
    }

    public class JDataGrid : DataGrid
    {
        private DateTime lastAdd_;

        public Panel ItemsHost =>
            (Panel)typeof(MultiSelector).InvokeMember("ItemsHost",
                BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance,
                null, this, null);

        public ScrollViewer ScrollViewer => this.GetScrollViewer();
        public ScrollBar ScrollBar => ScrollViewer.GetScrollBar();
        public bool IsScrollEnd => ScrollBar.Value == ScrollBar.Maximum;

        public JDataGridItemInfo NewItemInfo(object _item, DependencyObject _container = null, int _index = -1)
        {
            return new JDataGridItemInfo(_item, _container, _index).Refresh(ItemContainerGenerator);
        }

        public JDataGridItemInfo ItemInfoFromContainer(DependencyObject _container)
        {
            return NewItemInfo(ItemContainerGenerator.ItemFromContainer(_container), _container, ItemContainerGenerator.IndexFromContainer(_container));
        }

        public JDataGridItemInfo ItemInfoFromIndex(int _index)
        {
            return (_index >= 0) ? NewItemInfo(Items[_index], ItemContainerGenerator.ContainerFromIndex(_index), _index) : null;
        }

        // return an ItemInfo like the input one, but owned by this ItemsControl
        public JDataGridItemInfo LeaseItemInfo(JDataGridItemInfo _info, bool _ensureIndex = false)
        {
            // if the original has index data, it's already good enough
            if (_info.Index < 0)
            {
                // otherwise create a new info from the original's item
                _info = NewItemInfo(_info.Item);
                if (_ensureIndex && _info.Index < 0)
                {
                    _info.Index = Items.IndexOf(_info.Item);
                }
            }

            return _info;
        }

        protected override void OnKeyDown(KeyEventArgs _e)
        {
            MethodInfo onTabKeyDown = GetType().BaseType.GetMethod("OnTabKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo onEnterKeyDown = GetType().BaseType.GetMethod("OnEnterKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo onArrowKeyDown = GetType().BaseType.GetMethod("OnArrowKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo onHomeOrEndKeyDown = GetType().BaseType.GetMethod("OnHomeOrEndKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo onPageUpOrDownKeyDown = GetType().BaseType.GetMethod("OnPageUpOrDownKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);

            switch (_e.Key)
            {
                case Key.Tab:
                    onTabKeyDown.Invoke(this, new object[] { _e });
                    break;
                case Key.Enter:
                    onEnterKeyDown.Invoke(this, new object[] { _e });
                    break;
                case Key.Left:
                case Key.Right:
                case Key.Up:
                    onArrowKeyDown.Invoke(this, new object[] { _e });
                    break;
                case Key.Down:
                    // 아랫 방향키만 base.PageDown 방식처럼 처리함
                    OnArrowDownKeyDown();
                    _e.Handled = true;
                    break;

                case Key.Home:
                case Key.End:
                    onHomeOrEndKeyDown.Invoke(this, new object[] { _e });
                    break;

                case Key.PageUp:
                case Key.PageDown:
                    onPageUpOrDownKeyDown.Invoke(this, new object[] { _e });
                    break;
                default:
                    base.OnKeyDown(_e);
                    break;
            }
        }

        public object BringItemIntoView(JDataGridItemInfo _info)
        {
            FrameworkElement element = _info.Container as FrameworkElement;
            if (element != null)
            {
                element.BringIntoView();
            }
            else if ((_info = LeaseItemInfo(_info, true)).Index >= 0)
            {
                // We might be virtualized, try to de-virtualize the item.
                // Note: There is opportunity here to make a public OM.
                //
                // Call UpdateLayout first, in case there is a pending Measure
                // that replaces the ItemsHost with a different panel.   We should
                // forward the request to the correct panel, of course.
                UpdateLayout();

                VirtualizingPanel itemsHost = ItemsHost as VirtualizingPanel;
                if (itemsHost != null)
                {
                    itemsHost.BringIndexIntoViewPublic(_info.Index);
                }
            }

            return null;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs _e)
        {
            if (_e.Action == NotifyCollectionChangedAction.Add)
            {
                lastAdd_ = DateTime.Now;
            }

            base.OnItemsChanged(_e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs _e)
        {
            if (lastAdd_ + TimeSpan.FromMilliseconds(200) >= DateTime.Now)
                return;

            base.OnPreviewKeyDown(_e);
        }

        protected void OnArrowDownKeyDown()
        {
            if (SelectedIndex < 0)
                return;

            JDataGridItemInfo targetInfo = ItemInfoFromIndex(SelectedIndex + 1);
            DataGridColumn currentColumn = CurrentColumn;

            BringItemIntoView(targetInfo);
            CurrentItem = targetInfo.Item;
            SelectedItem = CurrentItem;
        }
    }
}
