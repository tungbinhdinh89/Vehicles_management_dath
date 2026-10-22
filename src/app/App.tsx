import { RouterProvider } from "react-router"
import { router } from "./routes"

export default function ApplicationRouter() {
  return <RouterProvider router={router} />
}
