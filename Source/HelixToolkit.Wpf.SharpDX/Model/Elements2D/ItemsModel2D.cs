using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    /// <summary>
    /// D2D项集合控件。
    /// </summary>
    public class ItemsModel2D : Canvas2D
    {
        #region ItemTemplate

        /// <summary>
        /// Gets or sets the <see cref="ItemTemplate"/> value.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ItemTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                                nameof(ItemTemplate),
                                typeof(DataTemplate),
                                typeof(ItemsModel2D),
                                new PropertyMetadata(null, ItemTemplatePropertyChanged));

        private static void ItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsModel2D)d).OnItemTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="ItemTemplate"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="ItemTemplate"/>.</param>
        /// <param name="newValue">New value of <see cref="ItemTemplate"/>.</param>
        protected virtual void OnItemTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {

        }

        #endregion

        #region ItemsSource

        /// <summary>
        /// Gets or sets the <see cref="ItemsSource"/> value.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                                nameof(ItemsSource),
                                typeof(IEnumerable),
                                typeof(ItemsModel2D),
                                new PropertyMetadata(null, ItemsSourcePropertyChanged));

        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsModel2D)d).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private readonly Dictionary<object, Element2D> m_ElementDict = new Dictionary<object, Element2D>();

        /// <summary>
        /// Called when <see cref="ItemsSource"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="ItemsSource"/>.</param>
        /// <param name="newValue">New value of <see cref="ItemsSource"/>.</param>
        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue is INotifyCollectionChanged o)
            {
                o.CollectionChanged -= ItemsModel2D_CollectionChanged;
                m_ElementDict.Clear();
                DetachChildren(Children);
                Children.Clear();
            }

            if (newValue == null)
            {
                return;
            }

            if (newValue is INotifyCollectionChanged n)
            {
                n.CollectionChanged += ItemsModel2D_CollectionChanged;
            }

            if (ItemTemplate == null)
            {
                foreach (var item in ItemsSource)
                {
                    if (item is Element2D model)
                    {
                        Children.Add(model);
                        m_ElementDict.Add(item, model);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot create a {nameof(Element2D)} from ItemTemplate.");
                    }
                }
            }
            else
            {
                foreach (var item in this.ItemsSource)
                {
                    if (ItemTemplate.LoadContent() is Element2D model)
                    {
                        model.DataContext = item;
                        Children.Add(model);
                        m_ElementDict.Add(item, model);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot create a {nameof(Element2D)} from ItemTemplate.");
                    }
                }
            }
        }

        private void ItemsModel2D_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (m_ElementDict.TryGetValue(item, out Element2D element))
                            {
                                Children.Remove(element);
                                m_ElementDict.Remove(item);
                            }
                        }
                        InvalidateRender();
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    m_ElementDict.Clear();
                    DetachChildren(Children);
                    break;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (ItemsSource != null)
                    {
                        if (ItemTemplate == null)
                        {
                            foreach (var item in this.ItemsSource)
                            {
                                if (item is Element2D model)
                                {
                                    Children.Add(model);
                                    m_ElementDict.Add(item, model);
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Cannot create a {nameof(Element2D)} from ItemTemplate.");
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in ItemsSource)
                            {
                                if (ItemTemplate.LoadContent() is Element2D model)
                                {
                                    model.DataContext = item;
                                    Children.Add(model);
                                    m_ElementDict.Add(item, model);
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Cannot create a {nameof(Element2D)} from ItemTemplate.");
                                }
                            }
                        }
                    }
                    InvalidateRender();
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        if (ItemTemplate != null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                if (ItemTemplate.LoadContent() is Element2D model)
                                {
                                    model.DataContext = item;
                                    Children.Add(model);
                                    m_ElementDict.Add(item, model);
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Cannot create a {nameof(Element2D)} from ItemTemplate.");
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in e.NewItems)
                            {
                                if (item is Element2D model)
                                {
                                    Children.Add(model);
                                    m_ElementDict.Add(item, model);
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Cannot create a {nameof(Element2D)} from ItemTemplate.");
                                }
                            }
                        }
                    }
                    break;
            }
        }

        #endregion

        #region ItemTemplateSelector

        /// <summary>
        /// Gets or sets the <see cref="ItemTemplateSelector"/> value.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ItemTemplateSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(
                                nameof(ItemTemplateSelector),
                                typeof(DataTemplateSelector),
                                typeof(ItemsModel2D),
                                new PropertyMetadata(null, ItemTemplateSelectorPropertyChanged));

        private static void ItemTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsModel2D)d).OnItemTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="ItemTemplateSelector"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="ItemTemplateSelector"/>.</param>
        /// <param name="newValue">New value of <see cref="ItemTemplateSelector"/>.</param>
        protected virtual void OnItemTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue)
        {

        }

        #endregion
    }
}
