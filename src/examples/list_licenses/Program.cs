
namespace DurangoInteropDotnet.examples.list_licenses {
    public class Program {
        static void Main(string[] args) {
            var manager = new LicenseManager();
            manager.PrintLicenses();
        }
    }
}
