﻿using System.Linq;
using NUnit.Framework;
using VanillaTransformer.Configuration;
using VanillaTransformer.Tests.ValuesProviders;

namespace VanillaTransformer.Tests.CoreTest
{
    [TestFixture]
    public class TransformConfigurationReaderTests
    {
        [Test]
        public void should_be_able_to_read_transformations_from_file()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"" />
                                </transformationGroup>
                            </root>")
            };
            
            //ACT
            var result = configurationReader.ReadFromFile(testFilePath);

            //ASSERT
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual("aaa.pattern.xml",result[0].PatternFilePath);
            Assert.IsNotNull(result[0].ValuesProvider);
            Assert.AreEqual("output.xml",result[0].OutputFilePath);
        }

        [Test]
        public void should_be_able_to_read_many_transformation_for_single_pattern()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"" />
                                    <transformation values=""aaa1.values.xml"" output=""output1.xml"" />
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var result = configurationReader.ReadFromFile(testFilePath);

            //ASSERT
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);
        }


        [Test]
        public void should_be_able_to_read_inline_values_from_transformation_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation output=""output.xml"">
                                        <values>
                                            <Val1>AAA</Val1>
                                        </values>                                    
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);
            var transformationToTest = transformConfigurations.First();

            //ACT
            var values = transformationToTest.ValuesProvider.GetValues();


            //ASSERT
            Assert.IsNotNull(values);
            Assert.IsTrue(values.ContainsKey("Val1"));
            Assert.AreEqual(values["Val1"], "AAA");
        }  
        
        
        [Test]
        public void should_be_able_to_read_post_transformation_from_root_node_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <postTransformations>
                                    <add name=""ReFormatXML"" />
                                </postTransformations>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"" />
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.IsNotNull(transformationToTest.PostTransformations);
            Assert.AreEqual(1, transformationToTest.PostTransformations.Count);
            Assert.AreEqual("ReFormatXML", transformationToTest.PostTransformations.First().Name);
        }


        [Test]
        public void should_be_able_to_read_post_transformation_from_group_node_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                     <postTransformations>
                                        <add name=""ReFormatXML"" />
                                    </postTransformations>
                                    <transformation values=""aaa.values.xml"" output=""output.xml"" />
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.IsNotNull(transformationToTest.PostTransformations);
            Assert.AreEqual(1, transformationToTest.PostTransformations.Count);
            Assert.AreEqual("ReFormatXML", transformationToTest.PostTransformations.First().Name);
        }

        [Test]
        public void should_be_able_to_read_post_transformation_from_transformation_node_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"">
                                        <postTransformations>
                                            <add name=""ReFormatXML"" />
                                        </postTransformations>
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.IsNotNull(transformationToTest.PostTransformations);
            Assert.AreEqual(1, transformationToTest.PostTransformations.Count);
            Assert.AreEqual("ReFormatXML", transformationToTest.PostTransformations.First().Name);
        }
        
        [Test]
        public void should_be_able_to_extend_post_transformation_on_lower_level_of_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <postTransformations>
                                    <add name=""StripXMLComments"" />
                                </postTransformations>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"">
                                        <postTransformations>
                                            <add name=""ReFormatXML"" />
                                        </postTransformations>
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.IsNotNull(transformationToTest.PostTransformations);
            Assert.AreEqual(2, transformationToTest.PostTransformations.Count);
            Assert.AreEqual("StripXMLComments", transformationToTest.PostTransformations[0].Name);
            Assert.AreEqual("ReFormatXML", transformationToTest.PostTransformations[1].Name);
        } 
        
        [Test]
        public void should_be_able_to_supress_post_transformation_on_lower_level_of_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <postTransformations>
                                    <add name=""StripXMLComments"" />
                                    <add name=""ReFormatXML"" />
                                </postTransformations>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"">
                                        <postTransformations>
                                            <remove name=""StripXMLComments"" />
                                        </postTransformations>
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.IsNotNull(transformationToTest.PostTransformations);
            Assert.AreEqual(1, transformationToTest.PostTransformations.Count);
            Assert.AreEqual("ReFormatXML", transformationToTest.PostTransformations[0].Name);
        } 
        
        [Test]
        public void should_be_able_to_supress_all_post_transformation_on_lower_level_of_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <postTransformations>
                                    <add name=""StripXMLComments"" />
                                    <add name=""ReFormatXML"" />
                                </postTransformations>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"">
                                        <postTransformations>
                                            <clear />
                                        </postTransformations>
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.IsNotNull(transformationToTest.PostTransformations);
            Assert.AreEqual(0, transformationToTest.PostTransformations.Count);
        }
    }
}
