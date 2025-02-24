namespace tests
{
    using models;
    using Xunit;

    public class RfpTest
    {
        private readonly RfpService rfpServices = new RfpService();

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
            // Vérifie que la liste ne contient pas de RFP dont la date limite est dans le passé
            Assert.DoesNotContain(result, rfp => rfp.DeadlineDate <= DateTime.Today);
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
            // Vérifie que le RFP attendu est dans le résultat
            Assert.Contains(result, rfp => rfp.Uuid == rfpToBeReturned.Uuid && rfp.DeadlineDate == rfpToBeReturned.DeadlineDate);
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
            // Vérifie que la liste n'est pas vide et contient les RFP dont la date limite est dans le futur
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, rfp => Assert.True(rfp.DeadlineDate >= DateTime.Today));
        }

        [Fact]
        public void Test_filterRrfpDeadlineNotReachedYet_should_return_rfp_with_no_deadline()
        {
            // Arrange
            var rfpToBeReturned = new RFP { Uuid = "2" }; // Pas de deadline
            var rfps = new List<RFP> { rfpToBeReturned };

            // Act
            var result = rfpServices.FilterRfpDeadlineNotReachedYet(rfps);

            // Assert
            // Vérifie que le RFP attendu est dans le résultat
            Assert.Contains(result, rfp => rfp.Uuid == rfpToBeReturned.Uuid);
        }
    }
}
