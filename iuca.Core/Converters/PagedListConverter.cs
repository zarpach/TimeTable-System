using AutoMapper;
using iuca.Application.Models;
using System.Collections.Generic;

namespace iuca.Application.Converters
{
    public class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>> where TSource : class where TDestination : class
    {
        public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination, ResolutionContext context)
        {
            var collection = context.Mapper.Map<List<TSource>, List<TDestination>>(source);

            return new PagedList<TDestination>(collection, source.Metadata.TotalCount, source.Metadata.CurrentPage, source.Metadata.PageSize);
        }
    }
}
