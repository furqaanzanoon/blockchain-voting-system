import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid,
} from "recharts";

interface Result {
  candidate: string;
  votes: number;
}

interface ResultsChartProps {
  data: Result[];
}

export default function ResultsChart({ data }: ResultsChartProps) {
  return (
    <ResponsiveContainer width="100%" height="100%">
      <BarChart data={data} margin={{ top: 20, right: 30, left: 0, bottom: 5 }}>
        <defs>
          <linearGradient id="voteGradient" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0%" stopColor="#22d3ee" stopOpacity={1} />
            <stop offset="100%" stopColor="#0284c7" stopOpacity={0.4} />
          </linearGradient>
        </defs>
        <CartesianGrid stroke="#1e293b" strokeDasharray="3 3" vertical={false} />
        <XAxis
          dataKey="candidate"
          stroke="#64748b"
          tick={{ fill: "#94a3b8", fontSize: 13 }}
          axisLine={{ stroke: "#334155" }}
          tickLine={{ stroke: "#334155" }}
        />
        <YAxis
          stroke="#64748b"
          tick={{ fill: "#94a3b8", fontSize: 13 }}
          axisLine={{ stroke: "#334155" }}
          tickLine={{ stroke: "#334155" }}
          allowDecimals={false}
        />
        <Tooltip
          contentStyle={{
            backgroundColor: "#0f172a",
            border: "1px solid #334155",
            borderRadius: "12px",
            color: "#fff",
          }}
          itemStyle={{ color: "#22d3ee" }}
          labelStyle={{ fontWeight: "bold", color: "#94a3b8" }}
        />
        <Bar
          dataKey="votes"
          fill="url(#voteGradient)"
          radius={[8, 8, 0, 0]}
          maxBarSize={60}
        />
      </BarChart>
    </ResponsiveContainer>
  );
}
