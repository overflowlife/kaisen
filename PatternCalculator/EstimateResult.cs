namespace PatternCalculator
{
    public struct EstimateResult
    {
        public int activePat;
        public double activeLife;
        public double passivePat;
        public double passiveLife;

        public EstimateResult(int activeEstPat, double activeEstLife, double passiveEstPat, double passiveEstLife)
        {
            this.activePat = activeEstPat;
            this.activeLife = activeEstLife;
            this.passivePat = passiveEstPat;
            this.passiveLife = passiveEstLife;
        }
    }
}