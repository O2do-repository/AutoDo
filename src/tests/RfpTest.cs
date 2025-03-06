namespace tests
{

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
                new RFP { RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174000") , DeadlineDate = DateTime.Today.AddDays(-1) }, // Obsolète
                new RFP { RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174002"), DeadlineDate = DateTime.Today.AddDays(5) }   // Valide
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
            var rfpToBeReturned = new RFP { RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174002"), DeadlineDate = DateTime.Today }; // Valide
            var rfps = new List<RFP> { rfpToBeReturned };

            // Act
            var result = rfpServices.FilterRfpDeadlineNotReachedYet(rfps);

            // Assert
            // Vérifie que le RFP attendu est dans le résultat
            Assert.Contains(result, rfp => rfp.RFPUuid == rfpToBeReturned.RFPUuid && rfp.DeadlineDate == rfpToBeReturned.DeadlineDate);
        }

        [Fact]
        public void Test_filterRrfpDeadlineNotReachedYet_should_return_rfp_with_deadline_in_the_future()
        {
            // Arrange
            var rfps = new List<RFP>
            {
                new RFP { RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174000"), DeadlineDate = DateTime.Today.AddDays(5) },
                new RFP { RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174002"), DeadlineDate = DateTime.Today.AddDays(10) }
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
            var rfpToBeReturned = new RFP { RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174002") }; // Pas de deadline
            var rfps = new List<RFP> { rfpToBeReturned };

            // Act
            var result = rfpServices.FilterRfpDeadlineNotReachedYet(rfps);

            // Assert
            // Vérifie que le RFP attendu est dans le résultat
            Assert.Contains(result, rfp => rfp.RFPUuid == rfpToBeReturned.RFPUuid);
        }
    }
}
