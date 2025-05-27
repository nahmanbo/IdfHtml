namespace IdfOperation.Organizations
{
    public abstract class Organization
    {
        private readonly DateTime _establishmentDate;
        private string _currentCommander;

        //====================================
        protected Organization(DateTime establishmentDate, string currentCommander)
        {
            _establishmentDate = establishmentDate;
            _currentCommander = currentCommander;
        }

        //--------------------------------------------------------------
        public DateTime GetEstablishmentDate()
        {
            return _establishmentDate;
        }

        //--------------------------------------------------------------
        public string GetCommander()
        {
            return _currentCommander;
        }

        //--------------------------------------------------------------
        public void ChangeCommander(string newCommander)
        {
            _currentCommander = newCommander;
        }

        //--------------------------------------------------------------
        public abstract string GetInfo();
    }
}