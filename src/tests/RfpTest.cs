namespace tests;

using FluentAssertions;
using models;
using Xunit;


public class RfpTest
{
    private List<RFP> filterRrfpDeadlineNotReachedYet(List<RFP> rfps)
    {
        return rfps.Where(data => data.DeadlineDate.CompareTo(DateTime.Today) >= 0).ToList();
    }

    [Fact]
    public void ShouldNotReturnObsoleteRfp()
    {
        // Arrange
        var rfps = new List<RFP>
        {
            new RFP { Uuid = "1", DeadlineDate = DateTime.Today.AddDays(-1) }, // Obsolète
            new RFP { Uuid = "2", DeadlineDate = DateTime.Today.AddDays(5) }   // Valide
        };

        // Act
        var result = filterRrfpDeadlineNotReachedYet(rfps);

        // Assert
        result.Should().NotContain(rfp => rfp.DeadlineDate <= DateTime.Today);
    }

    [Fact]
    public void ShouldReturnListRfp()
    {
        // Arrange
        var rfps = new List<RFP>
        {
            new RFP { Uuid = "1", DeadlineDate = DateTime.Today.AddDays(5) },
            new RFP { Uuid = "2", DeadlineDate = DateTime.Today.AddDays(10) }
        };

        // Act
        var result = filterRrfpDeadlineNotReachedYet(rfps);

        // Assert
        result.Should().NotBeEmpty().And.HaveCount(2);
    }
        [Fact]
    public void Test_filterRrfpDeadlineNotReachedYet_should_return_rfp_with_deadline_is_today()
    {

        var rfp_to_be_returned = new RFP { Uuid = "2", DeadlineDate = DateTime.Today }; // Valide
        // Arrange
        var rfps = new List<RFP>
        {
            rfp_to_be_returned
        };

        // Act
        var result = filterRrfpDeadlineNotReachedYet(rfps);

        // Assert
        result.Should().Contain(rfp_to_be_returned);
    }

    [Fact]
    public void Test_filterRrfpDeadlineNotReachedYet_should_return_rfp_with_no_deadline()
    {

        var rfp_to_be_returned = new RFP { Uuid = "2" }; // Valide
        // Arrange
        var rfps = new List<RFP>
        {
            rfp_to_be_returned
        };

        // Act
        var result = filterRrfpDeadlineNotReachedYet(rfps);

        // Assert
        result.Should().Contain(rfp_to_be_returned);
    }
    

}


