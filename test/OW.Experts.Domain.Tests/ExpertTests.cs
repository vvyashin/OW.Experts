using System;
using System.Collections.Generic;
using Domain.Infrastructure.Extensions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Domain.Infrastructure.Tests;

namespace Domain.Tests
{
    [TestFixture]
    public class ExpertTests
    {
        /// <summary>
        /// Объект - подделка сессии.
        /// </summary>
        /// <remarks>Для каждого теста создается новый экземпляр</remarks>
        SessionOfExperts _sessionFake;

        [SetUp]
        public void SessionFakeSetup()
        {
            _sessionFake = Substitute.For<SessionOfExperts>();
        }

        private Expert CreateExpert(string name = "name")
        {
            return new Expert(name, _sessionFake);
        }

        private class ExpertWithCollections : Expert
        {
            public ExpertWithCollections(string name, SessionOfExperts session,
                IEnumerable<Association> associations, IEnumerable<Relation> relations)
                : base(name, session)
            {
                this._associations.AddRange(associations);
                this._relations.AddRange(relations);
            }
        }

        [Test]
        public void ReplaceAllAssociation_IfAssociationsIsNull_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => CreateExpert().ReplaceAllAssociations(null));
        }

        [Test]
        public void ReplaceAllAssociation_ClearAllAssociationsAndAddNew()
        {
            var expert = CreateExpert(new[]
                {
                    Substitute.For<Association>(),
                    Substitute.For<Association>(),
                    Substitute.For<Association>()
                });
            expert.ReplaceAllAssociations(new List<string>() {"newAssociation1", "newAssociation2"});

            expert.Associations.Should().BeEquivalentTo(new []
            {
                new { Expert = expert, Name = "newAssociation1" },
                new { Expert = expert, Name = "newAssociation2" }
            }, opt => opt.ExcludingMissingMembers());
        }

        private Expert CreateExpert(IEnumerable<Association> associations)
        {
            return new ExpertWithCollections("name", _sessionFake,
                associations, new Relation[0]);
        }

        [Test]
        public void SetTypeForAssociation_IdIsEmpty_Throw()
        {
            var expert = CreateExpert();

            Assert.Throws<ArgumentException>(
                () => expert.SetTypeForAssociation(Guid.Empty, Substitute.For<NotionType>(), null));
        }

        [Test]
        public void SetTypeForAssociation_AssociationWithTheIdDoesNotExist_Throw()
        {
            var expert = CreateExpert();

            Assert.Throws<InvalidOperationException>(() => expert.SetTypeForAssociation(
                GeneratedGuids.First, Substitute.For<NotionType>(), null));
        }

        [Test]
        public void SetTypeForAssociation_AssociationWithTheIdExists_SetTypes()
        {
            var associationId = GeneratedGuids.First;
            var notionType = Substitute.For<NotionType>();

            var association = Substitute.For<Association>();
            association.Id.Returns(associationId);
            var expert = CreateExpert(new[]
            {
                association
            });

            expert.SetTypeForAssociation(associationId, notionType, "offerType");
            
            association.Received(requiredNumberOfCalls: 1).UpdateTypes(Arg.Is(notionType), Arg.Is("offerType"));
        }

        [Test]
        public void TestGenerateRelations()
        {
            var nodes = new[]
            {
                Substitute.For<Node>(),
                Substitute.For<Node>()
            };
            var expert = CreateExpert();

            expert.GenerateRelations(nodes);

            expert.Relations.Should().BeEquivalentTo(new []
            {
                new { Source = nodes[0], Destination = nodes[1], Expert = expert },
                new { Source = nodes[1], Destination = nodes[0], Expert = expert }
            }, opt => opt.ExcludingMissingMembers());
        }
        
        private Expert CreateExpert(IEnumerable<Relation> relations)
        {
            _sessionFake = Substitute.For<SessionOfExperts>();

            return new ExpertWithCollections("name", _sessionFake,
                new Association[0], relations);
        }

        [Test]
        public void GetNextRelationPair_IfThereAreNotRelationsThatHadNotChosen_ReturnNull()
        {
            var relation = Substitute.For<Relation>();
            relation.IsChosen.Returns(ChosenState.HadChosen);
            var expert = CreateExpert(relations: new[]
            {
                relation
            });

            var result = expert.GetNextRelationPair();

            result.Should().BeNull();
        }

        [Test]
        public void GetNextRelationPair_IfThereAreRelationsThatHadNotChosen_ReturnPair()
        {
            var expert = CreateExpert();

            expert.GenerateRelations(new[]
            {
                new Node("left", Substitute.For<NotionType>()),
                new Node("rigth", Substitute.For<NotionType>())
            });

            var result = expert.GetNextRelationPair();

            new[] { result.Item1, result.Item2}
                .Should().BeEquivalentTo(expert.Relations);
        }

        [Test]
        public void SetTypeForRelation_IdIsEmpty_Throw()
        {
            var expert = CreateExpert();

            Assert.Throws<ArgumentException>(
                () => expert.SetTypesForRelation(Guid.Empty, Substitute.For<RelationType>().Enumerate(), null));
        }

        [Test]
        public void SetTypeForRelation_RelationWithTheIdDoesNotExist_Throw()
        {
            var expert = CreateExpert();

            Assert.Throws<InvalidOperationException>(() => expert.SetTypesForRelation(
                GeneratedGuids.First, Substitute.For<RelationType>().Enumerate(), null));
        }

        [Test]
        public void SetTypeForRelation_RelationWithTheIdExists_SetTypes()
        {
            var relationId = GeneratedGuids.First;
            var relationTypes = new[] {Substitute.For<RelationType>()};

            var relation = Substitute.For<Relation>();
            relation.Id.Returns(relationId);
            var expert = CreateExpert(new[] { relation });

            expert.SetTypesForRelation(relationId, relationTypes, "offerType");

            relation.Received(requiredNumberOfCalls: 1).UpdateTypes(Arg.Is(relationTypes), Arg.Is("offerType"));
        }

        [Test]
        public void TestToString()
        {
            var expert = CreateExpert("Name");

            var actual = expert.ToString();

            actual.Should().Be($"Name - Сессия: {_sessionFake.ToString()}");
        }

        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.MakingAssociations)]
        public void FinishPhase_SetLastCompletedPhaseAsCurrentPhaseOfSession(SessionPhase currentPhase)
        {
            var expert = CreateExpert();
            SetCurrentSessionPhase(currentPhase);

            expert.FinishCurrentPhase();

            expert.LastCompletedPhase.Should().Be(currentPhase);
        }

        [TestCase(SessionPhase.MakingAssociations, null)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes, SessionPhase.MakingAssociations)]
        public void IsPhaseCompleted_CurrentSessionPhaseDoesNotEqualLastCompletedPhase_ReturnFalse(
            SessionPhase currentPhase, SessionPhase? lastCompletedPhase)
        {
            var expert = CreateExpert();
            SetCurrentSessionPhase(currentPhase);
            expert.LastCompletedPhase = lastCompletedPhase;

            expert.IsPhaseCompleted.Should().Be(false);
        }

        [Test]
        public void IsPhaseCompleted_CurrentSessionPhaseEqualsLastCompletedPhase_ReturnTrue()
        {
            var expert = CreateExpert();
            SetCurrentSessionPhase(SessionPhase.MakingAssociations);
            expert.LastCompletedPhase = SessionPhase.MakingAssociations;

            expert.IsPhaseCompleted.Should().Be(true);
        }

        private void SetCurrentSessionPhase(SessionPhase currentPhase)
        {
            _sessionFake.CurrentPhase.Returns(currentPhase);
        }
    }
}
