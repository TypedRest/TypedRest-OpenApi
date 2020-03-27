using Xunit;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpInterfaceFacts : CSharpTypeFactsBase
    {
        [Fact]
        public void GeneratesCorrectCode()
        {
            var myInterface = new CSharpIdentifier(ns: "Namespace1", name: "MyInterface");
            var baseInterface = new CSharpIdentifier(ns: "Namespace2", name: "BaseInterface");
            var endpointInterface = new CSharpIdentifier("TypedRest.Endpoints.Generic", "ICollectionEndpoint")
            {
                TypeArguments = {new CSharpIdentifier(ns: "Models", name: "MyModel")}
            };

            Assert(new CSharpInterface(myInterface)
            {
                Description = "My interface\nDetails",
                Interfaces = {baseInterface},
                Properties =
                {
                    new CSharpProperty(endpointInterface, "MyProperty")
                    {
                        Description = "My property"
                    }
                }
            }, @"using Models;
using Namespace2;
using TypedRest.Endpoints.Generic;

namespace Namespace1
{
    /// <summary>
    /// My interface
    /// Details
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode(""TypedRest.OpenApi"", ""1.0.0"")]
    public partial interface MyInterface : BaseInterface
    {
        /// <summary>
        /// My property
        /// </summary>
        ICollectionEndpoint<MyModel> MyProperty
        {
            get;
        }
    }
}");
        }
    }
}
