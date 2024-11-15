using CMS.ContactManagement;
using CMS.DataEngine;
using Kentico.Web.Mvc;
using Moq;
using TrainingGuides.Web.Features.Membership;
using TrainingGuides.Web.Features.Membership.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Membership.Services;

public class MemberContactServiceTests
{
    private readonly Mock<IInfoProvider<ContactInfo>> contactInfoProviderMock;
    private readonly Mock<ICookieAccessor> cookieAccessorMock;
    private readonly Mock<ICurrentContactProvider> currentContactProviderMock;

    private const string GIVEN_NAME = "John";
    private const string FAMILY_NAME = "Doe";
    private const string EMAIL_1 = "JohnDoe@localhost.local";
    private const string EMAIL_2 = "NotJohnDoe@localhost.local";

    public MemberContactServiceTests()
    {
        contactInfoProviderMock = new Mock<IInfoProvider<ContactInfo>>();
        cookieAccessorMock = new Mock<ICookieAccessor>();
        currentContactProviderMock = new Mock<ICurrentContactProvider>();
    }

    private ContactInfo BuildSampleContactInfo(string firstName, string lastName, string email) =>
        new()
        {
            ContactFirstName = firstName,
            ContactLastName = lastName,
            ContactEmail = email,
            ContactID = 1,
            ContactGUID = Guid.NewGuid()
        };

    private GuidesMember BuildSampleGuidesMember(string givenName, string familyName, string email) =>
        new()
        {
            GivenName = givenName,
            FamilyName = familyName,
            Email = email,
            Id = 1,
        };

    [Fact]
    public void TransferMemberFieldsToContact_Does_Not_Overwrite_Email()
    {
        // Arrange
        var memberContactService = new MemberContactService(
            contactInfoProvider: contactInfoProviderMock.Object,
            cookieAccessor: cookieAccessorMock.Object,
            currentContactProvider: currentContactProviderMock.Object);

        var guidesMember = BuildSampleGuidesMember(
            givenName: GIVEN_NAME,
            familyName: FAMILY_NAME,
            email: EMAIL_1);

        var contact = BuildSampleContactInfo(
            firstName: GIVEN_NAME,
            lastName: FAMILY_NAME,
            email: EMAIL_2);

        var newContact = memberContactService.TransferMemberFieldsToContact(guidesMember, contact);

        Assert.NotEqual(guidesMember.Email, newContact.ContactEmail);
    }

    [Fact]
    public void TransferMemberFieldsToContact_Overwrites_Blank_Email()
    {
        // Arrange
        var memberContactService = new MemberContactService(
            contactInfoProvider: contactInfoProviderMock.Object,
            cookieAccessor: cookieAccessorMock.Object,
            currentContactProvider: currentContactProviderMock.Object);

        var guidesMember = BuildSampleGuidesMember(
            givenName: GIVEN_NAME,
            familyName: FAMILY_NAME,
            email: EMAIL_1);

        var contact = BuildSampleContactInfo(
            firstName: GIVEN_NAME,
            lastName: FAMILY_NAME,
            email: string.Empty);

        var newContact = memberContactService.TransferMemberFieldsToContact(guidesMember, contact);

        Assert.Equal(guidesMember.Email, newContact.ContactEmail);
    }

    [Fact]
    public void TransferMemberFieldsToContact_Overwrites_FirstName()
    {
        // Arrange
        var memberContactService = new MemberContactService(
            contactInfoProvider: contactInfoProviderMock.Object,
            cookieAccessor: cookieAccessorMock.Object,
            currentContactProvider: currentContactProviderMock.Object);

        var guidesMember = BuildSampleGuidesMember(
            givenName: GIVEN_NAME,
            familyName: FAMILY_NAME,
            email: EMAIL_1);

        var contact = BuildSampleContactInfo(
            firstName: GIVEN_NAME,
            lastName: FAMILY_NAME,
            email: string.Empty);

        var newContact = memberContactService.TransferMemberFieldsToContact(guidesMember, contact);

        Assert.Equal(guidesMember.GivenName, newContact.ContactFirstName);
    }

    [Fact]
    public void TransferMemberFieldsToContact_Overwrites_LastName()
    {
        // Arrange
        var memberContactService = new MemberContactService(
            contactInfoProvider: contactInfoProviderMock.Object,
            cookieAccessor: cookieAccessorMock.Object,
            currentContactProvider: currentContactProviderMock.Object);

        var guidesMember = BuildSampleGuidesMember(
            givenName: GIVEN_NAME,
            familyName: FAMILY_NAME,
            email: EMAIL_1);

        var contact = BuildSampleContactInfo(
            firstName: GIVEN_NAME,
            lastName: FAMILY_NAME,
            email: string.Empty);

        var newContact = memberContactService.TransferMemberFieldsToContact(guidesMember, contact);

        Assert.Equal(guidesMember.FamilyName, newContact.ContactLastName);
    }
}