// useState digunakan untuk mengelola state pada component.
import { useState } from "react";
import { Link } from "react-router";
// Mengimpor reusable input component.
import FormField from "../components/forms/FormField";

// Mengimpor fungsi komunikasi ke Login API.
import { loginUser } from "../services/authApi";

// Halaman utama login aplikasi KodaStore.
export default function LoginPage() {
  // Menyimpan email dan password yang diisi user.
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  // Menentukan apakah request Login sedang diproses.
  const [isLoading, setIsLoading] = useState(false);

  // Menyimpan pesan error dari validasi atau API.
  const [errorMessage, setErrorMessage] = useState("");

  // Menyimpan pesan ketika verifikasi credential berhasil.
  const [successMessage, setSuccessMessage] = useState("");

  // Menyimpan data hasil verifikasi untuk ditampilkan saja.
  // Ini belum merupakan authenticated session.
  const [verifiedUser, setVerifiedUser] = useState(null);

  // Menangani perubahan email atau password.
  function handleChange(event) {
    // Mengambil nama input dan nilai terbarunya.
    const { name, value } = event.target;

    // Memperbarui field berdasarkan atribut name.
    setFormData((previous) => ({
      ...previous,
      [name]: value,
    }));

    // Menghapus pesan lama jika user mengubah input.
    setErrorMessage("");
    setSuccessMessage("");

    // Menghapus hasil verifikasi sebelumnya.
    setVerifiedUser(null);
  }

  // Menangani submit form login.
  async function handleSubmit(event) {
    // Mencegah browser melakukan reload halaman.
    event.preventDefault();

    // Mencegah submit tambahan saat request berlangsung.
    if (isLoading) return;

    // Mereset pesan dan hasil verifikasi sebelumnya.
    setErrorMessage("");
    setSuccessMessage("");
    setVerifiedUser(null);

    // Memastikan email dan password sudah diisi.
    if (!formData.email.trim() || !formData.password) {
      setErrorMessage("Email dan password wajib diisi.");
      return;
    }

    // Mengaktifkan loading sebelum memanggil API.
    setIsLoading(true);

    try {
      // Mengirim email dan password ke service API.
      const response = await loginUser(formData);

      // Menyimpan hasil verifikasi untuk tampilan.
      setVerifiedUser(response.user);

      // Menampilkan pesan bahwa credential berhasil diverifikasi.
      setSuccessMessage("Email dan password cocok.");

      // Menghapus password dari state setelah request berhasil.
      setFormData((previous) => ({
        ...previous,
        password: "",
      }));
    } catch (error) {
      // Mengambil pesan error dari API service.
      setErrorMessage(
        error instanceof Error
          ? error.message
          : "Terjadi kesalahan saat login.",
      );
    } finally {
      // Loading dihentikan ketika request sudah selesai.
      setIsLoading(false);
    }
  }

  return (
    <main className="min-h-screen items-center bg-slate-950 px-4 py-10 text-slate-900">
      <div className="mx-auto items-center grid min-h-[85vh] max-w-5xl overflow-hidden rounded-3xl bg-white shadow-2xl md:grid-cols-2">
        <section className="flex items-center p-7 sm:p-12">
          <div className="w-full">
            <p className="text-sm font-bold tracking-widest text-orange-600">
              KODASTORE
            </p>

            <h2 className="mt-3 text-3xl font-bold text-slate-900">Sign in</h2>

            <p className="mt-3 text-sm text-slate-500">
              Masukkan email dan password akun KodaStore.
            </p>

            <form onSubmit={handleSubmit} className="mt-8 space-y-5">
              <FormField
                id="email"
                label="Email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                autoComplete="email"
                placeholder="email@example.com"
              />

              <FormField
                id="password"
                label="Password"
                type="password"
                value={formData.password}
                onChange={handleChange}
                autoComplete="current-password"
                placeholder="Masukkan password"
              />

              <button
                type="submit"
                disabled={
                  isLoading || !formData.email.trim() || !formData.password
                }
                className="w-full rounded-xl bg-orange-600 px-5 py-3 font-semibold text-white transition hover:bg-orange-700 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {isLoading ? "Memverifikasi..." : "Login"}
              </button>
            </form>
            <Link to="/register">Register</Link>

            {errorMessage && (
              <div
                role="alert"
                className="mt-5 rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700"
              >
                {errorMessage}
              </div>
            )}

            {successMessage && (
              <div
                role="status"
                className="mt-5 rounded-xl border border-green-200 bg-green-50 p-4 text-sm text-green-800"
              >
                <p className="font-semibold">Verifikasi berhasil</p>

                <p className="mt-1">{successMessage}</p>

                {verifiedUser && (
                  <p className="mt-2">User: {verifiedUser.displayName}</p>
                )}
              </div>
            )}
          </div>
        </section>
      </div>
    </main>
  );
}
