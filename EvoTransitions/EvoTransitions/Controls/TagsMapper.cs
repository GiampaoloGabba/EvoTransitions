using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace EvoTransitions.Controls
{
    public class TagMapper : ITagMapper
    {
        private readonly Lazy<List<TagMap>> _tagStack = new Lazy<List<TagMap>>(() => new List<TagMap>());

        public IReadOnlyList<TagMap> MapStack => _tagStack.Value;

        public IReadOnlyList<Tags> GetMap(Page page, int group = 0)
        {
            return MapStack.Where(x => x.PageId == page.Id)
                           .Select(x => x.Tags.Where(y=>y.Group == group).ToList())
                           .FirstOrDefault() ?? new List<Tags>();
        }

        public void Add(Page page, int tag, int group, int viewId)
        {
            var tagMap = _tagStack.Value.FirstOrDefault(x => x.PageId == page.Id);

            if (tagMap == null)
            {
                _tagStack.Value.Add(
                    new TagMap
                    {
                        PageId = page.Id,
                        Tags = new List<Tags>{CreateTag(tag, group, viewId) }
                    }
                );
            } else if (tagMap.Tags.All(x => x.Tag != tag))
            {
                tagMap.Tags.Add(CreateTag(tag, group, viewId));
            }
            else
            {
                //the tag exists but are we sure group and viewId are changed?
                var currentTag = tagMap.Tags.FirstOrDefault(x => x.Tag == tag);
                if (currentTag != null)
                {
                    if (currentTag.Group != group)
                        currentTag.Group = group;

                    if (currentTag.ViewId != viewId)
                        currentTag.ViewId = viewId;
                } 
            }
        }

        public void Remove(Page page)
        {
            _tagStack.Value.Remove(_tagStack.Value.FirstOrDefault(x => x.PageId == page.Id));
        }

        private Tags CreateTag(int tag, int group, int viewId)
        {
            return new Tags
            {
                Tag    = tag,
                Group  = group,
                ViewId = viewId
            };
        }
    }
}
