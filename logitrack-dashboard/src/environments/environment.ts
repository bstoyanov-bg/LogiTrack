export const environment = {
  production: false,

  // ShipmentService REST API
  shipmentServiceBaseUrl: 'https://localhost:7251/api',
  // DriverService (gRPC has no /api, but health is exposed via REST endpoints)
  driverServiceBaseUrl: 'https://localhost:5085',
  // REST endpoints (DriverController)
  driverApiBaseUrl: 'https://localhost:5085/api',
  // SignalR Hub endpoint
  signalRHubUrl: 'https://localhost:7251/hub/shipments'
};