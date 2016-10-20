﻿using System;
using NUnit.Framework;
using System.Threading;
using PubnubApi;
using MockServer;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class WhenGetRequestServerTime : TestHarness
    {
        private static ManualResetEvent mreTime = new ManualResetEvent(false);
        private static bool timeReceived = false;
        private static string currentUnitTestCase = "";
        private static long expectedTime = 14725889985315301;
        private static Pubnub pubnub = null;

        private Server server;
        private UnitTestLog unitLog;

        [TestFixtureSetUp]
        public void Init()
        {
            unitLog = new Tests.UnitTestLog();
            unitLog.LogLevel = MockServer.LoggingMethod.Level.Verbose;
            server = new Server(new Uri("https://" + PubnubCommon.StubOrign));
            MockServer.LoggingMethod.MockServerLog = unitLog;
            server.Start();
        }

        [TestFixtureTearDown]
        public void Exit()
        {
            server.Stop();
        }

        [Test]
        public void ThenItShouldReturnTimeStamp()
        {
            if (PubnubCommon.EnableStubTest)
            {
                Assert.Ignore("EnableStubTest is True.");
                return;
            }

            currentUnitTestCase = "ThenItShouldReturnTimeStamp";
            timeReceived = false;
            mreTime = new ManualResetEvent(false);

            PNConfiguration config = new PNConfiguration()
            {
                PublishKey = PubnubCommon.PublishKey,
                SubscribeKey = PubnubCommon.SubscribeKey,
                Uuid = "mytestuuid",
                Secure = false
            };

            pubnub = this.createPubNubInstance(config);

            expectedTime = 14725889985315301;

            pubnub.Time().Async(new TimeResult());
                
            mreTime.WaitOne(310 * 1000);

            pubnub.EndPendingRequests();
            pubnub = null;

            Assert.IsTrue(timeReceived, "time() Failed");
        }

        [Test]
        public void ThenItShouldReturnTimeStampWithSSL()
        {
            currentUnitTestCase = "ThenItShouldReturnTimeStampWithSSL";

            timeReceived = false;
            mreTime = new ManualResetEvent(false);

            PNConfiguration config = new PNConfiguration()
            {
                PublishKey = PubnubCommon.PublishKey,
                SubscribeKey = PubnubCommon.SubscribeKey,
                Uuid = "mytestuuid",
            };

            pubnub = this.createPubNubInstance(config);

            string expected = "[14725889985315301]";

            server.AddRequest(new Request()
                    .WithMethod("GET")
                    .WithPath("/time/0")
                    .WithParameter("pnsdk", PubnubCommon.EncodedSDK)
                    .WithParameter("requestid", "myRequestId")
                    .WithParameter("timestamp", "1356998400")
                    .WithParameter("uuid", config.Uuid)
                    .WithResponse(expected)
                    .WithStatusCode(System.Net.HttpStatusCode.OK));

            expectedTime = 14725889985315301;

            pubnub.Time().Async(new TimeResult());

            mreTime.WaitOne(310 * 1000);

            pubnub.EndPendingRequests();
            pubnub = null;

            Assert.IsTrue(timeReceived, "time() with SSL Failed");
        }

        [Test]
        public void ThenWithProxyItShouldReturnTimeStamp()
        {
            if (PubnubCommon.EnableStubTest)
            {
                Assert.Ignore("EnableStubTest is True.");
                return;
            }

            currentUnitTestCase = "ThenWithProxyItShouldReturnTimeStamp";

            bool proxyConfigured = true;

            IPubnubProxy proxy = new PubnubProxy();
            proxy.Server = "test.pandu.com";
            proxy.Port = 808;
            proxy.UserName = "tuvpnfreeproxy";
            proxy.Password = "Rx8zW78k";

            timeReceived = false;
            mreTime = new ManualResetEvent(false);

            PNConfiguration config = new PNConfiguration()
            {
                PublishKey = PubnubCommon.PublishKey,
                SubscribeKey = PubnubCommon.SubscribeKey,
                Uuid = "mytestuuid",
                PNProxy = proxy,
                Secure=false
            };

            pubnub = this.createPubNubInstance(config);

            expectedTime = 14725889985315301;

            if (proxyConfigured)
            {
                pubnub.Time().Async(new TimeResult());

                mreTime.WaitOne(310 * 1000);

                pubnub.EndPendingRequests();

                pubnub.PubnubUnitTest = null;
                pubnub = null;

                Assert.IsTrue(timeReceived, "time() Failed");
            }
            else
            {
                Assert.Ignore("Proxy setup not configured. After setup Set proxyConfigured to true");
            }
        }

        [Test]
        public void ThenWithProxyItShouldReturnTimeStampWithSSL()
        {
            bool proxyConfigured = true;

            IPubnubProxy proxy = new PubnubProxy();
            proxy.Server = "test.pandu.com";
            proxy.Port = 808;
            proxy.UserName = "tuvpnfreeproxy";
            proxy.Password = "Rx8zW78k";

            timeReceived = false;
            mreTime = new ManualResetEvent(false);

            PNConfiguration config = new PNConfiguration()
            {
                PublishKey = PubnubCommon.PublishKey,
                SubscribeKey = PubnubCommon.SubscribeKey,
                Uuid = "mytestuuid",
                PNProxy = proxy
            };

            pubnub = this.createPubNubInstance(config);

            string expected = "[14725889985315301]";

            server.AddRequest(new Request()
                    .WithMethod("GET")
                    .WithPath("/time/0")
                    .WithParameter("pnsdk", PubnubCommon.EncodedSDK)
                    .WithParameter("requestid", "myRequestId")
                    .WithParameter("timestamp", "1356998400")
                    .WithParameter("uuid", config.Uuid)
                    .WithResponse(expected)
                    .WithStatusCode(System.Net.HttpStatusCode.OK));

            long expectedTime = 14725889985315301;

            if (proxyConfigured)
            {
                pubnub.Time().Async(new TimeResult());

                mreTime.WaitOne(310 * 1000);

                pubnub.EndPendingRequests();

                pubnub.PubnubUnitTest = null;
                pubnub = null;

                Assert.IsTrue(timeReceived, "time() with SSL through proxy Failed");
            }
            else
            {
                Assert.Ignore("Proxy setup for SSL not configured. After setup Set proxyConfigured to true");
            }
        }

        [Test]
        public void TranslateDateTimeToUnixTime()
        {
            //Test for 26th June 2012 GMT
            DateTime dt = new DateTime(2012, 6, 26, 0, 0, 0, DateTimeKind.Utc);
            long nanoSecondTime = Pubnub.TranslateDateTimeToPubnubUnixNanoSeconds(dt);
            Assert.True(13406688000000000 == nanoSecondTime);
        }

        [Test]
        public void TranslateUnixTimeToDateTime()
        {
            //Test for 26th June 2012 GMT
            DateTime expectedDate = new DateTime(2012, 6, 26, 0, 0, 0, DateTimeKind.Utc);
            DateTime actualDate = Pubnub.TranslatePubnubUnixNanoSecondsToDateTime(13406688000000000);
            Assert.True(expectedDate == actualDate);
        }

        public class TimeResult : PNCallback<PNTimeResult>
        {
            public override void OnResponse(PNTimeResult result, PNStatus status)
            {
                try
                {
                    Console.WriteLine("PNStatus={0}", pubnub.JsonPluggableLibrary.SerializeToJsonString(status));

                    if (result != null)
                    {
                        Console.WriteLine(pubnub.JsonPluggableLibrary.SerializeToJsonString(result));

                        if (status.StatusCode == 200 && status.Error == false)
                        {
                            if (PubnubCommon.EnableStubTest)
                            {
                                if (expectedTime == result.Timetoken)
                                {
                                    timeReceived = true;
                                }
                            }
                            else if (result.Timetoken > 0)
                            {
                                timeReceived = true;
                            }
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    mreTime.Set();
                }

            }
        };

    }
}