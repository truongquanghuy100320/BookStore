namespace BookStore.Extention
{
    [Serializable]
    public  class UserLogin
    {
        
        public int CustomerId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int Role { get; set; }
        public string Desciption { get; set; }


    }
}
