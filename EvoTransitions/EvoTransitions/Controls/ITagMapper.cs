using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace EvoTransitions.Controls
{
    public interface ITagMapper
    {
        IReadOnlyList<TagMap> MapStack { get; }
        IReadOnlyList<Tags> GetMap(Page page, int group = 0);
        void Add(Page page, int tag, int group, int viewId);
        void Remove(Page page);
    }

    public class TagMap
    {
        public Guid PageId { get; set; }
        public List<Tags> Tags { get; set; }
    }

    public class Tags
    {
        public int Tag { get; set; }
        public int Group { get; set; }
        public int ViewId { get; set; }
    }
}
