﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Spring.Context.Support;
using TradeHub.Common.Core.Constants;
using TradeHub.Common.Core.DomainModels;
using TradeHub.StrategyEngine.OrderExecution;
using TradeHubGui.Common;
using TradeHubGui.Common.Constants;
using TradeHubGui.Common.Models;
using TradeHubGui.Dashboard.Services;

namespace TradeHubGui.Dashboard.Tests.Integration
{
    [TestFixture]
    public class OrderExecutionControllerTests
    {
        private OrderExecutionController _orderExecutionController;

        [SetUp]
        public void StartUp()
        {
            _orderExecutionController = ContextRegistry.GetContext()["OrderExecutionController"] as OrderExecutionController;
        }

        [TearDown]
        public void TearDown()
        {
            _orderExecutionController.Stop();
        }

        [Test]
        [Category("Integration")]
        public void RequestNewConnection_SendRequestToServer_ReceiveLogon()
        {
            Thread.Sleep(9000);
            Provider provider = new Provider();
            provider.ConnectionStatus = ConnectionStatus.Disconnected;
            provider.ProviderName = OrderExecutionProvider.Simulated;

            // Rasie event to request connection
            EventSystem.Publish<Provider>(provider);

            Thread.Sleep(9000);

            Assert.IsTrue(provider.ConnectionStatus.Equals(ConnectionStatus.Connected));
        }

        [Test]
        [Category("Integration")]
        public void RequestNewMarketOrder_SendRequestToServer_ReceiveOrderAcceptance()
        {
            Thread.Sleep(5000);
            Provider provider = new Provider();
            provider.ProviderType = ProviderType.OrderExecution;
            provider.ConnectionStatus = ConnectionStatus.Disconnected;
            provider.ProviderName = OrderExecutionProvider.Simulated;

            // Rasie event to request connection
            EventSystem.Publish<Provider>(provider);

            Thread.Sleep(5000);

            Assert.IsTrue(provider.ConnectionStatus.Equals(ConnectionStatus.Connected));

            OrderDetails orderDetails = new OrderDetails();
            orderDetails.ID = "01";
            orderDetails.Type = OrderType.Market;
            orderDetails.Quantity = 10;
            orderDetails.Security = new Security() {Symbol = "AAPL"};
            orderDetails.Side = OrderSide.BUY;
            orderDetails.Provider = OrderExecutionProvider.Simulated;

            // Create Order Request
            OrderRequest orderRequest = new OrderRequest(orderDetails, OrderRequestType.New);

            // Add Order Details object to Provider's Order Map
            provider.OrdersCollection.Add(orderDetails);

            // Rasie event to request order
            EventSystem.Publish<OrderRequest>(orderRequest);

            Thread.Sleep(5000);

            Assert.IsTrue(orderDetails.Status.Equals(OrderStatus.SUBMITTED));
        }

        [Test]
        [Category("Integration")]
        public void RequestNewLimitOrder_SendRequestToServer_ReceiveOrderAcceptance()
        {
            Thread.Sleep(5000);
            Provider provider = new Provider();
            provider.ProviderType = ProviderType.OrderExecution;
            provider.ConnectionStatus = ConnectionStatus.Disconnected;
            provider.ProviderName = OrderExecutionProvider.Simulated;

            // Rasie event to request connection
            EventSystem.Publish<Provider>(provider);

            Thread.Sleep(5000);

            Assert.IsTrue(provider.ConnectionStatus.Equals(ConnectionStatus.Connected));

            OrderDetails orderDetails = new OrderDetails();
            orderDetails.ID = "01";
            orderDetails.Type = OrderType.Limit;
            orderDetails.Price = 44.56M;
            orderDetails.Quantity = 10;
            orderDetails.Security = new Security() { Symbol = "AAPL" };
            orderDetails.Side = OrderSide.BUY;
            orderDetails.Provider = OrderExecutionProvider.Simulated;

            // Create Order Request
            OrderRequest orderRequest = new OrderRequest(orderDetails, OrderRequestType.New);

            // Add Order Details object to Provider's Order Map
            provider.OrdersCollection.Add(orderDetails);

            // Rasie event to request order
            EventSystem.Publish<OrderRequest>(orderRequest);

            Thread.Sleep(5000);

            Assert.IsTrue(orderDetails.Status.Equals(OrderStatus.SUBMITTED));
        }

        [Test]
        [Category("Integration")]
        public void RequestNewMarketOrderForExecution_SendRequestToServer_ReceiveOrderExecution()
        {
            Thread.Sleep(5000);
            Provider provider = new Provider();
            provider.ProviderType = ProviderType.OrderExecution;
            provider.ConnectionStatus = ConnectionStatus.Disconnected;
            provider.ProviderName = OrderExecutionProvider.Simulated;

            // Rasie event to request connection
            EventSystem.Publish<Provider>(provider);

            Thread.Sleep(5000);

            Assert.IsTrue(provider.ConnectionStatus.Equals(ConnectionStatus.Connected));

            OrderDetails orderDetails = new OrderDetails();
            orderDetails.ID = "01";
            orderDetails.Type = OrderType.Market;
            orderDetails.Quantity = 10;
            orderDetails.Security = new Security() { Symbol = "AAPL" };
            orderDetails.Side = OrderSide.BUY;
            orderDetails.Provider = OrderExecutionProvider.Simulated;

            // Create Order Request
            OrderRequest orderRequest = new OrderRequest(orderDetails, OrderRequestType.New);

            // Add Order Details object to Provider's Order Map
            provider.OrdersCollection.Add(orderDetails);

            // Rasie event to request order
            EventSystem.Publish<OrderRequest>(orderRequest);

            Thread.Sleep(9000);

            Assert.IsTrue(orderDetails.Status.Equals(OrderStatus.EXECUTED));
            Assert.IsTrue(orderDetails.FillDetails.Count.Equals(1));
        }

        [Test]
        [Category("Integration")]
        public void RequestNewLimitOrderForExecution_SendRequestToServer_ReceiveOrderExecution()
        {
            Thread.Sleep(5000);
            Provider provider = new Provider();
            provider.ProviderType = ProviderType.OrderExecution;
            provider.ConnectionStatus = ConnectionStatus.Disconnected;
            provider.ProviderName = OrderExecutionProvider.Simulated;

            // Rasie event to request connection
            EventSystem.Publish<Provider>(provider);

            Thread.Sleep(5000);

            Assert.IsTrue(provider.ConnectionStatus.Equals(ConnectionStatus.Connected));

            OrderDetails orderDetails = new OrderDetails();
            orderDetails.ID = "01";
            orderDetails.Type = OrderType.Limit;
            orderDetails.Price = 44.76M;
            orderDetails.Quantity = 10;
            orderDetails.Security = new Security() { Symbol = "AAPL" };
            orderDetails.Side = OrderSide.BUY;
            orderDetails.Provider = OrderExecutionProvider.Simulated;

            // Create Order Request
            OrderRequest orderRequest = new OrderRequest(orderDetails, OrderRequestType.New);

            // Add Order Details object to Provider's Order Map
            provider.OrdersCollection.Add(orderDetails);

            // Rasie event to request order
            EventSystem.Publish<OrderRequest>(orderRequest);

            Thread.Sleep(9000);

            Assert.IsTrue(orderDetails.Status.Equals(OrderStatus.EXECUTED));
            Assert.IsTrue(orderDetails.FillDetails.Count.Equals(1));
        }

        [Test]
        [Category("Integration")]
        public void RequestOrderCancellation_SendNewOrderRequest_ReceiveOrderAcceptance_SendOrderCancellation_ReceiveOrderCancellation()
        {
            Thread.Sleep(5000);
            Provider provider = new Provider();
            provider.ProviderType = ProviderType.OrderExecution;
            provider.ConnectionStatus = ConnectionStatus.Disconnected;
            provider.ProviderName = OrderExecutionProvider.Simulated;

            // Rasie event to request connection
            EventSystem.Publish<Provider>(provider);

            Thread.Sleep(5000);

            Assert.IsTrue(provider.ConnectionStatus.Equals(ConnectionStatus.Connected));

            OrderDetails orderDetails = new OrderDetails();
            orderDetails.ID = "01";
            orderDetails.Type = OrderType.Limit;
            orderDetails.Quantity = 10;
            orderDetails.Price = 45.98M;
            orderDetails.Security = new Security() { Symbol = "AAPL" };
            orderDetails.Side = OrderSide.BUY;
            orderDetails.Provider = OrderExecutionProvider.Simulated;

            {
                // Create Order Request
                OrderRequest orderRequest = new OrderRequest(orderDetails, OrderRequestType.New);

                // Add Order Details object to Provider's Order Map
                provider.OrdersCollection.Add(orderDetails);

                // Rasie event to request order
                EventSystem.Publish<OrderRequest>(orderRequest);
            }

            Thread.Sleep(5000);

            Assert.IsTrue(orderDetails.Status.Equals(OrderStatus.SUBMITTED));
            
            {
                // Create Cancel Order Request
                OrderRequest cancelOrderRequest = new OrderRequest(orderDetails, OrderRequestType.Cancel);

                // Rasie event to request order
                EventSystem.Publish<OrderRequest>(cancelOrderRequest);
            }

            Thread.Sleep(5000);

            Assert.IsTrue(orderDetails.Status.Equals(OrderStatus.CANCELLED));
        }
    }
}
