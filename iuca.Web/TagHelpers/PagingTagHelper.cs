using iuca.Application.DTO.Courses;
using iuca.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace iuca.Web.TagHelpers
{
    public class PagingTagHelper: TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PagingTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PagedListMetadata Metadata { get; set; }
        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";

            // набор ссылок будет представлять список ul
            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");
            tag.AddCssClass("d-inline-block");

            // формируем три ссылки - на текущую, предыдущую и следующую
            TagBuilder currentItem = CreateTag(Metadata.CurrentPage, urlHelper, Metadata.CurrentPage.ToString());

            // создаем ссылку на предыдущую страницу, если она есть
            if (Metadata.HasPrevious)
            {
                int PreviousPageNumber = Metadata.CurrentPage - 1;

                if (PreviousPageNumber > 1)
                {
                    //First page number
                    TagBuilder firstItem = CreateTag(1, urlHelper, "1");
                    tag.InnerHtml.AppendHtml(firstItem);
                }

                //Previous page button
                TagBuilder nextBtn = CreateTag(PreviousPageNumber, urlHelper, "<");
                tag.InnerHtml.AppendHtml(nextBtn);

                //Previous page number
                TagBuilder prevItem = CreateTag(PreviousPageNumber, urlHelper, PreviousPageNumber.ToString());
                tag.InnerHtml.AppendHtml(prevItem);
            }

            tag.InnerHtml.AppendHtml(currentItem);
            // создаем ссылку на следующую страницу, если она есть
            if (Metadata.HasNext)
            {
                int NextPageNumber = Metadata.CurrentPage + 1;

                //Next page number
                TagBuilder nextItem = CreateTag(NextPageNumber, urlHelper, NextPageNumber.ToString());
                tag.InnerHtml.AppendHtml(nextItem);

                if (NextPageNumber <= Metadata.TotalPages)
                {
                    //Next page button
                    TagBuilder nextBtn = CreateTag(NextPageNumber, urlHelper, ">");
                    tag.InnerHtml.AppendHtml(nextBtn);

                    if (NextPageNumber < Metadata.TotalPages)
                    {
                        //Last page number
                        TagBuilder lastItem = CreateTag(Metadata.TotalPages, urlHelper, Metadata.TotalPages.ToString());
                        tag.InnerHtml.AppendHtml(lastItem);
                    }
                }
            }
            output.Content.AppendHtml(tag);
        }

        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper, string innerHtml)
        {
            TagBuilder item = new TagBuilder("li");
            item.AddCssClass("d-inline-block");
            TagBuilder link = new TagBuilder("a");
            if (pageNumber == this.Metadata.CurrentPage)
            {
                item.AddCssClass("active");
            }
            else
            {
                PageUrlValues["pageNumber"] = pageNumber;
                link.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            }
            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(innerHtml);
            item.InnerHtml.AppendHtml(link);
            return item;
        }
    }
}
