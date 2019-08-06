using Moq;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    public class PatternFactsBase<TPattern>
        where TPattern : IPattern, new()
    {
        protected IEndpoint TryGetEndpoint(PathTree tree, EndpointList childMatches = null)
        {
            var patternMatcherMock = new Mock<IPatternMatcher>();
            patternMatcherMock.Setup(x => x.GetEndpoints(tree))
                              .Returns(childMatches ?? new EndpointList());

            return new TPattern().TryGetEndpoint(tree, patternMatcherMock.Object);
        }
    }
}
