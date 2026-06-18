import { useEffect, useState } from "react";
import api from "../services/api";
import { FaUserShield, FaUsers } from "react-icons/fa";
import { useToast } from "../context/ToastContext";

interface Officer {
  userId: string;
  fullName: string;
  email: string;
  isVerified: boolean;
}

export default function ElectionOfficers() {
  const [form, setForm] = useState({
    fullName: "",
    email: "",
    password: "",
  });
  const [loading, setLoading] = useState(false);
  const [officers, setOfficers] = useState<Officer[]>([]);
  const [fetching, setFetching] = useState(false);

  const { showToast } = useToast();

  const fetchOfficers = async () => {
    try {
      setFetching(true);
      const res = await api.get("/admin/officers");
      setOfficers(res.data || []);
    } catch (err) {
      console.error(err);
      showToast("Failed to load election officers", "error");
    } finally {
      setFetching(false);
    }
  };

  useEffect(() => {
    fetchOfficers();
  }, []);

  const createOfficer = async (event: React.FormEvent) => {
    event.preventDefault();

    try {
      setLoading(true);

      await api.post("/Admin/create-officer", form);

      showToast(
        "Election officer created successfully. OTP sent to their email.",
        "success"
      );

      setForm({
        fullName: "",
        email: "",
        password: "",
      });

      fetchOfficers();
    } catch (error: any) {
      showToast(
        error?.response?.data?.message || "Failed to create election officer",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h1 className="text-4xl font-bold mb-8">Election Officers</h1>

      <div className="bg-slate-900 rounded-3xl p-6 mb-8 shadow-lg border border-slate-800">
        <div className="flex items-center gap-3 mb-5">
          <FaUserShield size={28} className="text-cyan-400" />
          <h2 className="text-2xl font-semibold">Create Election Officer</h2>
        </div>

        <form onSubmit={createOfficer}>
          <div className="grid md:grid-cols-2 gap-4">
            <input
              type="text"
              required
              className="bg-slate-800 p-3 rounded-xl border border-slate-700 text-white"
              placeholder="Full Name"
              value={form.fullName}
              onChange={(event) =>
                setForm({
                  ...form,
                  fullName: event.target.value,
                })
              }
            />

            <input
              type="email"
              required
              className="bg-slate-800 p-3 rounded-xl border border-slate-700 text-white"
              placeholder="Email Address"
              value={form.email}
              onChange={(event) =>
                setForm({
                  ...form,
                  email: event.target.value,
                })
              }
            />

            <input
              type="password"
              required
              className="bg-slate-800 p-3 rounded-xl border border-slate-700 md:col-span-2 text-white"
              placeholder="Temporary Password"
              value={form.password}
              onChange={(event) =>
                setForm({
                  ...form,
                  password: event.target.value,
                })
              }
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="mt-5 bg-cyan-500 hover:bg-cyan-400 disabled:bg-slate-600 disabled:text-slate-300 text-black font-bold px-6 py-3 rounded-xl transition-all"
          >
            {loading ? "Creating Officer..." : "Create Officer"}
          </button>
        </form>
      </div>

      <div className="bg-slate-900 rounded-3xl p-6 border border-slate-800 shadow-lg mb-8">
        <div className="flex items-center gap-3 mb-6">
          <FaUsers className="text-cyan-400" size={24} />
          <h2 className="text-xl font-bold text-white">All Election Officers</h2>
        </div>

        {fetching && officers.length === 0 ? (
          <div className="text-center py-10 text-slate-500">
            <span className="animate-spin inline-block h-6 w-6 border-2 border-cyan-400 border-t-transparent rounded-full mr-2 align-middle"></span>
            Loading officers...
          </div>
        ) : officers.length === 0 ? (
          <div className="text-center py-10 text-slate-500 font-medium">
            No election officers registered.
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="border-b border-slate-800/80 text-slate-400 text-xs font-semibold uppercase tracking-wider">
                  <th className="py-4 px-4">Full Name</th>
                  <th className="py-4 px-4">Email Address</th>
                  <th className="py-4 px-4">Verification Status</th>
                </tr>
              </thead>
              <tbody>
                {officers.map((officer) => (
                  <tr
                    key={officer.userId}
                    className="border-b border-slate-800/60 hover:bg-slate-800/20 transition-all duration-200"
                  >
                    <td className="py-4 px-4 font-semibold text-white">
                      {officer.fullName}
                    </td>
                    <td className="py-4 px-4 text-slate-300">
                      {officer.email}
                    </td>
                    <td className="py-4 px-4">
                      {officer.isVerified ? (
                        <span className="px-3 py-1 rounded-full text-xs font-bold bg-green-500/10 text-green-400 border border-green-500/20">
                          OTP Verified
                        </span>
                      ) : (
                        <span className="px-3 py-1 rounded-full text-xs font-bold bg-amber-500/10 text-amber-400 border border-amber-500/20">
                          Pending OTP
                        </span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <div className="bg-slate-900 rounded-3xl p-6 text-slate-400 border border-slate-800">
        New election officers are created with the ElectionOfficer role and must verify their email OTP before logging in.
      </div>
    </div>
  );
}
