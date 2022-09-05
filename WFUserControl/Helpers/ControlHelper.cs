using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WFUserControl
{
    public static class ControlHelper
	{
		/// <summary>
		/// 获取指定类型的父级对象
		/// </summary>
		public static T FindParent<T>(DependencyObject child, Func<T, bool> findCondition = null) where T : DependencyObject
		{
			if (child == null)
			{
				return null;
			}
			DependencyObject parentObject = null;
			if (child is FrameworkElement || child is FrameworkContentElement)
			{
				parentObject = LogicalTreeHelper.GetParent(child);
			}
			if (parentObject == null)
			{
				if (child is Visual || child is Visual3D)
				{
					parentObject = VisualTreeHelper.GetParent(child);
				}
			}

			if (parentObject == null)
			{
				return null;
			}

			T parent = parentObject as T;
			if (parent != null)
			{
				if (findCondition != null)
				{
					bool isTheObject = findCondition(parent);
					if (isTheObject)
					{
						return parent;
					}
					return FindParent<T>(parentObject, findCondition);
				}
				return parent;
			}
			else
			{
				return FindParent<T>(parentObject, findCondition);
			}
		}

		/// <summary>
		/// 获取可视化树中，指定类型和名称的直接子级对象
		/// </summary>
		public static T FindChild<T>(DependencyObject parent, Func<T, bool> findCondition) where T : DependencyObject
		{
			if (parent == null)
			{
				return null;
			}

			if (parent is FrameworkElement || parent is FrameworkContentElement)
			{
				IEnumerable enumerable = LogicalTreeHelper.GetChildren(parent);
				IEnumerator enumerator = enumerable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current as T;
					if (current != null)
					{
						bool isFind = findCondition(current);
						if (isFind)
						{
							return current;
						}
					}
				}

			}

			if (parent is Visual || parent is Visual3D)
			{
				int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
				for (int i = 0; i < childrenCount; i++)
				{
					var child = VisualTreeHelper.GetChild(parent, i) as T;
					if (child != null)
					{
						bool isFind = findCondition(child);
						if (isFind)
						{
							return child;
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// 获取可视化树中，指定类型的子级对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parent"></param>
		/// <param name="findCondition"></param>
		/// <returns></returns>
		public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
		{
			if (parent == null)
			{
				return null;
			}

			if (parent is FrameworkElement || parent is FrameworkContentElement)
			{
				IEnumerable enumerable = LogicalTreeHelper.GetChildren(parent);
				IEnumerator enumerator = enumerable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is T current)
					{
						return current;
					}

					DependencyObject dependencyObject = enumerator.Current as DependencyObject;
					if (dependencyObject != null)
					{
						T result = FindChild<T>(dependencyObject);
						if (result != null)
						{
							return result;
						}
					}
				}
			}

			if (parent is Visual || parent is Visual3D)
			{
				int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
				for (int i = 0; i < childrenCount; i++)
				{
					DependencyObject dependencyObject = VisualTreeHelper.GetChild(parent, i);
					if (dependencyObject is T child)
					{
						return child;
					}

					if (dependencyObject != null)
					{
						T result = FindChild<T>(dependencyObject);
						if (result != null)
						{
							return result;
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// 获取可视化树中，指定类型和名称的子孙级对象, 对比子级对象的获取，这个进行了更深的前项遍历，执行效率比较低，但是更可靠。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parent"></param>
		/// <param name="findCondition"></param>
		/// <returns></returns>
		public static T FindDescendant<T>(DependencyObject parent, Func<T, bool> findCondition) where T : DependencyObject
		{
			if (parent == null)
			{
				return null;
			}

			if (parent is FrameworkElement || parent is FrameworkContentElement)
			{
				IEnumerable enumerable = LogicalTreeHelper.GetChildren(parent);
				IEnumerator enumerator = enumerable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current as T;
					if (current != null)
					{
						bool isFind = findCondition(current);
						if (isFind)
						{
							return current;
						}

					}

					DependencyObject dependencyObject = enumerator.Current as DependencyObject;
					if (dependencyObject != null)
					{
						T result = FindDescendant(dependencyObject, findCondition);
						if (result != null)
						{
							return result;
						}
					}
				}
			}

			if (parent is Visual || parent is Visual3D)
			{
				int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
				for (int i = 0; i < childrenCount; i++)
				{
					var child = VisualTreeHelper.GetChild(parent, i) as T;
					if (child != null)
					{
						bool isFind = findCondition(child);
						if (isFind)
						{
							return child;
						}
					}

					DependencyObject dependencyObject = child as DependencyObject;
					if (dependencyObject != null)
					{
						T result = FindDescendant(dependencyObject, findCondition);
						if (result != null)
						{
							return result;
						}
					}
				}
			}

			return null;
		}
	}
}
