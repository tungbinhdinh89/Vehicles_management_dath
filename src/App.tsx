import { useMemo } from "react"
import { CssBaseline, ThemeProvider } from "@mui/material"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { makeTheme } from "./theme"
import GarageProvider from "./store"
import ApplicationRouter from "./app/App"

const queryClient = new QueryClient({
  defaultOptions: { queries: { refetchOnWindowFocus: false } },
})

export default function App() {
  const theme = useMemo(makeTheme, [])
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <QueryClientProvider client={queryClient}>
        <GarageProvider>
          <ApplicationRouter />
        </GarageProvider>
      </QueryClientProvider>
    </ThemeProvider>
  )
}
