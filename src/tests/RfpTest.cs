

namespace tests;

using FluentAssertions;
using models;
using Xunit;

public class RfpTest
{
    private readonly RfpServices rfpServices = new RfpServices();

    [Fact]
    public void Test_filterRrfpDeadlineNotReachedYet_does_not_return_rfp_with_deadline_in_the_past()
    {
        // Arrange
        var rfps = new List<RFP>
        {
            new RFP { Uuid = "1", DeadlineDate = DateTime.Today.AddDays(-1) }, // Obsolète
            new RFP { Uuid = "2", DeadlineDate = DateTime.Today.AddDays(5) }   // Valide
        };

        // Act
        var result = rfpServices.FilterRfpDeadlineNotReachedYet(rfps);

        // Assert
        result.Should().NotContain(rfp => rfp.DeadlineDate <= DateTime.Today);
    }

    [Fact]
    public void Test_filterRrfpDeadlineNotReachedYet_should_return_rfp_with_deadline_today()
    {
        // Arrange
        var rfpToBeReturned = new RFP { Uuid = "2", DeadlineDate = DateTime.Today }; // Valide
        var rfps = new List<RFP> { rfpToBeReturned };

        // Act
        var result = rfpServices.FilterRfpDeadlineNotReachedYet(rfps);

        // Assert
        result.Should().Contain(rfpToBeReturned);
    }

    [Fact]
    public void Test_filterRrfpDeadlineNotReachedYet_should_return_rfp_with_deadline_in_the_future()
    {
        // Arrange
        var rfps = new List<RFP>
        {
            new RFP { Uuid = "1", DeadlineDate = DateTime.Today.AddDays(5) },
            new RFP { Uuid = "2", DeadlineDate = DateTime.Today.AddDays(10) }
        };

        // Act
        var result = rfpServices.FilterRfpDeadlineNotReachedYet(rfps);

        // Assert
        result.Should().NotBeEmpty()
          .And.HaveCount(2)
          .And.OnlyContain(rfp => rfp.DeadlineDate >= DateTime.Today);
    }

    [Fact]
    public void Test_filterRrfpDeadlineNotReachedYet_should_return_rfp_with_no_deadline()
    {

        var rfp_to_be_returned = new RFP { Uuid = "2" };
        // Arrange
        var rfps = new List<RFP>
        {
            rfp_to_be_returned
        };

        // Act
        var result = rfpServices.FilterRfpDeadlineNotReachedYet(rfps);

        // Assert
        result.Should().Contain(rfp_to_be_returned);
    }
}

