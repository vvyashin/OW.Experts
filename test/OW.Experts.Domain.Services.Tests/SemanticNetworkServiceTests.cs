using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using OW.Experts.Domain.Infrastructure.Extensions;
using OW.Experts.Test.Infrastructure;

namespace OW.Experts.Domain.Services.Tests
{
    [TestFixture]
    public class SemanticNetworkServiceTests
    {
        public INodeRepository FakeNodeRepository { get; set; }

        public IVergeRepository FakeVergeRepository { get; set; }

        public ITypeRepository<NotionType> FakeNotionTypeRepository { get; set; }

        public ITypeRepository<RelationType> FakeRelationTypeRepository { get; set; }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_CalculateVergeWeight()
        {
            #region arrange

            var serviceUnderTests = CreateServiceUnderTest();

            FakeNodeRepository.GetByNotionAndType(Arg.Any<string>(), Arg.Any<NotionType>()).Returns((Node)null);
            FakeVergeRepository.GetByNodesAndTypes(Arg.Any<Node>(), Arg.Any<Node>(), Arg.Any<RelationType>())
                .Returns((Verge)null);

            var nodeCandidates = new[]
            {
                new NodeCandidate { IsSaveAsNode = true, Notion = "notion", TotalExpert = 5, ExpertCount = 2 },
                new NodeCandidate { IsSaveAsNode = true, Notion = "notion2", TotalExpert = 5, ExpertCount = 3 }
            };
            var session = CreateFakeSession("baseNotion");

            #endregion

            serviceUnderTests.CreateSemanticNetworkFromNodeCandidates(nodeCandidates, session);

            FakeVergeRepository.Received(1).AddOrUpdate(
                Arg.Is<Verge>(
                    x =>
                        x.SourceNode.Notion == "baseNotion" && x.DestinationNode.Notion == "notion" &&
                        x.Weight == 40));
            FakeVergeRepository.Received(1).AddOrUpdate(
                Arg.Is<Verge>(
                    x =>
                        x.SourceNode.Notion == "baseNotion" && x.DestinationNode.Notion == "notion2" &&
                        x.Weight == 60));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_CreateNodeForRootAndForNodeCandidates()
        {
            #region arrange

            var serviceUnderTests = CreateServiceUnderTest();

            FakeNodeRepository.GetByNotionAndType(Arg.Any<string>(), Arg.Any<NotionType>()).Returns((Node)null);
            var generalType = ReturnsGeneralNotionTypeFromRepository();
            var notionTypeId = GeneratedGuids.First;
            var notionType = Substitute.For<NotionType>();
            FakeNotionTypeRepository.GetById(Arg.Is(notionTypeId)).Returns(notionType);

            var nodeCandidate = new NodeCandidate { IsSaveAsNode = true, Notion = "notion", TypeId = notionTypeId };
            var session = CreateFakeSession("baseNotion");

            #endregion

            serviceUnderTests.CreateSemanticNetworkFromNodeCandidates(nodeCandidate.Enumerate(), session);

            FakeNodeRepository.Received(1)
                .AddOrUpdate(Arg.Is<Node>(x => x.Notion == "baseNotion" && x.Type == generalType));
            FakeNodeRepository.Received(1).AddOrUpdate(Arg.Is<Node>(x => x.Notion == "notion" && x.Type == notionType));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_CreateVergesFromRootToEachNodeCandidateSavedAsNode()
        {
            #region arrange

            var serviceUnderTests = CreateServiceUnderTest();

            FakeNodeRepository.GetByNotionAndType(Arg.Any<string>(), Arg.Any<NotionType>()).Returns((Node)null);
            FakeVergeRepository.GetByNodesAndTypes(Arg.Any<Node>(), Arg.Any<Node>(), Arg.Any<RelationType>())
                .Returns((Verge)null);
            var generalRelationType = ReturnsGeneralRelationTypeFromRepository();

            var nodeCandidates = new[]
            {
                new NodeCandidate { IsSaveAsNode = true, Notion = "notion", ExpertCount = 1, TotalExpert = 5 },
                new NodeCandidate { IsSaveAsNode = false, Notion = "notion2" }
            };
            var session = CreateFakeSession("baseNotion");

            #endregion

            serviceUnderTests.CreateSemanticNetworkFromNodeCandidates(nodeCandidates, session);

            FakeVergeRepository.Received(1).AddOrUpdate(
                Arg.Is<Verge>(
                    x =>
                        x.SourceNode.Notion == "baseNotion" && x.DestinationNode.Notion == "notion" &&
                        x.Type == generalRelationType));
            FakeVergeRepository.Received(1).AddOrUpdate(
                Arg.Is<Verge>(
                    x =>
                        x.SourceNode.Notion == "notion" && x.DestinationNode.Notion == "baseNotion" &&
                        x.Type == generalRelationType));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_DtoIsNull_ArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(null, CreateFakeSession()));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_FilteredByIsSaveAsNode()
        {
            #region arrange

            var serviceUnderTests = CreateServiceUnderTest();

            FakeNodeRepository.GetByNotionAndType(Arg.Any<string>(), Arg.Any<NotionType>()).Returns((Node)null);
            var notionTypeId = GeneratedGuids.First;
            var notionType = Substitute.For<NotionType>();
            FakeNotionTypeRepository.GetById(Arg.Is(notionTypeId)).Returns(notionType);

            var nodeCandidates = new[]
            {
                new NodeCandidate { IsSaveAsNode = true, Notion = "notion", TypeId = notionTypeId },
                new NodeCandidate { IsSaveAsNode = false, Notion = "notion2", TypeId = notionTypeId }
            };
            var session = CreateFakeSession("baseNotion");

            #endregion

            serviceUnderTests.CreateSemanticNetworkFromNodeCandidates(nodeCandidates, session);

            FakeNodeRepository.Received(1).AddOrUpdate(Arg.Is<Node>(x => x.Notion == "notion" && x.Type == notionType));
            FakeNodeRepository.DidNotReceive()
                .AddOrUpdate(Arg.Is<Node>(x => x.Notion == "notion2" && x.Type == notionType));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_IfNodeExistsInRepo_UpdateIt()
        {
            #region arrange

            var serviceUnderTests = CreateServiceUnderTest();

            var generalType = ReturnsGeneralNotionTypeFromRepository();

            var notionTypeId = GeneratedGuids.First;
            var notionType = Substitute.For<NotionType>();
            FakeNotionTypeRepository.GetById(Arg.Is(notionTypeId)).Returns(notionType);

            var root = Substitute.For<Node>();
            FakeNodeRepository.GetByNotionAndType(Arg.Is("baseNotion"), Arg.Is(generalType)).Returns(root);

            var node = Substitute.For<Node>();
            FakeNodeRepository.GetByNotionAndType(Arg.Is("notion"), Arg.Is(notionType)).Returns(node);

            var nodeCandidate = new NodeCandidate { IsSaveAsNode = true, Notion = "notion", TypeId = GeneratedGuids.First };
            var session = CreateFakeSession("baseNotion");

            #endregion

            serviceUnderTests.CreateSemanticNetworkFromNodeCandidates(nodeCandidate.Enumerate(), session);

            FakeNodeRepository.Received(1).AddOrUpdate(Arg.Is(root));
            FakeNodeRepository.Received(1).AddOrUpdate(Arg.Is(node));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_IfVergeExists_UpdateItAndLinkSession()
        {
            #region arrange

            var serviceUnderTests = CreateServiceUnderTest();

            FakeNodeRepository.GetByNotionAndType(Arg.Any<string>(), Arg.Any<NotionType>()).Returns((Node)null);
            var verge = Substitute.For<Verge>();
            FakeVergeRepository.GetByNodesAndTypes(
                    Arg.Is<Node>(x => x.Notion == "source"),
                    Arg.Is<Node>(x => x.Notion == "destination"),
                    Arg.Any<RelationType>())
                .Returns(verge);
            var session = CreateFakeSession("source");

            var nodeCandidates = new[]
            {
                new NodeCandidate { IsSaveAsNode = true, Notion = "destination", TotalExpert = 5, ExpertCount = 2 }
            };

            #endregion

            serviceUnderTests.CreateSemanticNetworkFromNodeCandidates(nodeCandidates, session);

            verge.Received(1).UpdateWeightFromSession(Arg.Is(40), Arg.Is(session));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_LinkNodeAndSession()
        {
            #region arrange

            var serviceUnderTests = CreateServiceUnderTest();

            var node = Substitute.For<Node>();
            FakeNodeRepository.GetByNotionAndType(Arg.Is("baseNotion"), Arg.Any<NotionType>())
                .Returns(node);
            var session = CreateFakeSession("baseNotion");

            var nodeCandidates = new NodeCandidate[0];

            #endregion

            serviceUnderTests.CreateSemanticNetworkFromNodeCandidates(nodeCandidates, session);

            node.Received(1).AddSessionOfExperts(session);
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_SessionIsNull_ArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(new List<NodeCandidate>(), null));
        }

        [Test]
        public void GetNodesBySession_Anyway_ReturnNodesFromRepository()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = CreateFakeSession();
            var nodes = new List<Node>
            {
                Substitute.For<Node>(),
                Substitute.For<Node>()
            };
            FakeNodeRepository.GetBySession(Arg.Is(session)).Returns(nodes);

            var result = serviceUnderTest.GetNodesBySession(session);

            Assert.AreSame(result, nodes);
        }

        [Test]
        public void SaveRelationsAsVergesOfSemanticNetwork()
        {
            #region arrange

            var serviceUnderTests = CreateServiceUnderTest();

            FakeNodeRepository.GetByNotionAndType(Arg.Any<string>(), Arg.Any<NotionType>()).Returns((Node)null);
            FakeVergeRepository.GetByNodesAndTypes(Arg.Any<Node>(), Arg.Any<Node>(), Arg.Any<RelationType>())
                .Returns((Verge)null);

            var source = Substitute.For<Node>();
            var destination = Substitute.For<Node>();
            var type = Substitute.For<RelationType>();
            var groupedRelations = new[]
            {
                new GroupedRelation
                {
                    Source = source, Destination = destination, Type = type,
                    ExpertCount = 4, TotalExpectCount = 5
                }
            };
            var session = CreateFakeSession("baseNotion");

            #endregion

            serviceUnderTests.SaveRelationsAsVergesOfSemanticNetwork(groupedRelations, session);

            FakeVergeRepository.Received(1).AddOrUpdate(
                Arg.Is<Verge>(
                    x =>
                        x.SourceNode == source && x.DestinationNode == destination &&
                        x.Weight == 80));
        }

        [Test]
        public void SaveRelationsAsVergesOfSemanticNetwork_IfVergeExists_UpdateIt()
        {
            #region arrange

            var serviceUnderTests = CreateServiceUnderTest();

            var verge = Substitute.For<Verge>();
            var source = Substitute.For<Node>();
            var destination = Substitute.For<Node>();
            var type = Substitute.For<RelationType>();
            FakeVergeRepository.GetByNodesAndTypes(Arg.Is(source), Arg.Is(destination), Arg.Is(type)).Returns(verge);
            var session = CreateFakeSession();

            var groupedRelations = new[]
            {
                new GroupedRelation
                {
                    Source = source, Destination = destination, ExpertCount = 1,
                    TotalExpectCount = 5, Type = type
                }
            };

            #endregion

            serviceUnderTests.SaveRelationsAsVergesOfSemanticNetwork(groupedRelations, session);

            verge.Received(1).UpdateWeightFromSession(Arg.Is(20), Arg.Is(session));
        }

        [Test]
        public void SaveRelationsAsVergesOfSemanticNetwork_RelationCollectionIsNull_ArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.SaveRelationsAsVergesOfSemanticNetwork(null, CreateFakeSession()));
        }

        private SemanticNetworkService CreateServiceUnderTest()
        {
            FakeNodeRepository = Substitute.For<INodeRepository>();
            FakeVergeRepository = Substitute.For<IVergeRepository>();
            FakeNotionTypeRepository = Substitute.For<ITypeRepository<NotionType>>();
            FakeRelationTypeRepository = Substitute.For<ITypeRepository<RelationType>>();

            return new SemanticNetworkService(
                FakeNodeRepository,
                FakeVergeRepository,
                FakeNotionTypeRepository,
                FakeRelationTypeRepository);
        }

        private SessionOfExperts CreateFakeSession()
        {
            var session = Substitute.For<SessionOfExperts>();

            return session;
        }

        private SessionOfExperts CreateFakeSession(string notion)
        {
            var session = Substitute.For<SessionOfExperts>();
            session.BaseNotion.Returns(notion);

            return session;
        }

        private NotionType ReturnsGeneralNotionTypeFromRepository()
        {
            var notionType = Substitute.For<NotionType>();
            FakeNotionTypeRepository.GetGeneralType().Returns(notionType);

            return notionType;
        }

        private RelationType ReturnsGeneralRelationTypeFromRepository()
        {
            var relationType = Substitute.For<RelationType>();
            FakeRelationTypeRepository.GetGeneralType().Returns(relationType);

            return relationType;
        }
    }
}