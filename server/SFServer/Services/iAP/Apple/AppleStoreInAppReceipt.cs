using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services.iAP.Apple
{
    public class AppleStoreInAppReceipt
    {
        /// <summary>
        /// The time Apple customer support canceled a transaction, or the time an auto-renewable subscription plan was upgraded, in a date-time format similar to the ISO 8601. This field is only present for refunded transactions.
        /// </summary>
        public string cancellation_date { get; set; }

        /// <summary>
        /// The time Apple customer support canceled a transaction, or the time an auto-renewable subscription plan was upgraded, in UNIX epoch time format, in milliseconds. This field is only present for refunded transactions. Use this time format for processing dates. See cancellation_date_ms for more information.
        /// </summary>
        public string cancellation_date_ms { get; set; }

        /// <summary>
        /// The time Apple customer support canceled a transaction, or the time an auto-renewable subscription plan was upgraded, in the Pacific Time zone. This field is only present for refunded transactions.
        /// </summary>
        public string cancellation_date_pst { get; set; }

        /// <summary>
        /// The reason for a refunded transaction. When a customer cancels a transaction, the App Store gives them a refund and provides a value for this key. A value of “1” indicates that the customer canceled their transaction due to an actual or perceived issue within your app. A value of “0” indicates that the transaction was canceled for another reason; for example, if the customer made the purchase accidentally.
        /// Possible values: 1, 0
        /// </summary>
        public string cancellation_reason { get; set; }

        /// <summary>
        /// The time a subscription expires or when it will renew, in a date-time format similar to the ISO 8601.
        /// </summary>
        public string expires_date { get; set; }

        /// <summary>
        /// The time a subscription expires or when it will renew, in UNIX epoch time format, in milliseconds. Use this time format for processing dates. 
        /// </summary>
        public string expires_date_ms { get; set; }

        /// <summary>
        /// The time a subscription expires or when it will renew, in the Pacific Time zone.
        /// </summary>
        public string expires_date_pst { get; set; }

        /// <summary>
        /// An indicator of whether an auto-renewable subscription is in the introductory price period.
        /// </summary>
        public bool is_in_intro_offer_period { get; set; }

        /// <summary>
        /// An indication of whether a subscription is in the free trial period.
        /// </summary>
        public bool is_trial_period { get; set; }

        /// <summary>
        /// The time of the original in-app purchase, in a date-time format similar to ISO 8601.
        /// </summary>
        public string original_purchase_date { get; set; }

        /// <summary>
        /// The time of the original in-app purchase, in UNIX epoch time format, in milliseconds. For an auto-renewable subscription, this value indicates the date of the subscription's initial purchase. The original purchase date applies to all product types and remains the same in all transactions for the same product ID. This value corresponds to the original transaction’s transactionDate property in StoreKit. Use this time format for processing dates.
        /// </summary>
        public string original_purchase_date_ms { get; set; }

        /// <summary>
        /// The time of the original in-app purchase, in the Pacific Time zone.
        /// </summary>
        public string original_purchase_date_pst { get; set; }

        /// <summary>
        /// The transaction identifier of the original purchase.
        /// </summary>
        public string original_transaction_id { get; set; }

        /// <summary>
        /// The unique identifier of the product purchased. You provide this value when creating the product in App Store Connect, and it corresponds to the productIdentifier property of the SKPayment object stored in the transaction's payment property.
        /// </summary>
        public string product_id { get; set; }

        /// <summary>
        /// The identifier of the subscription offer redeemed by the user.
        /// </summary>
        public string promotional_offer_id { get; set; }

        /// <summary>
        /// The time the App Store charged the user's account for a purchased or restored product, or the time the App Store charged the user’s account for a subscription purchase or renewal after a lapse, in a date-time format similar to ISO 8601. 
        /// </summary>
        public string purchase_date { get; set; }

        /// <summary>
        /// For consumable, non-consumable, and non-renewing subscription products, the time the App Store charged the user's account for a purchased or restored product, in the UNIX epoch time format, in milliseconds. For auto-renewable subscriptions, the time the App Store charged the user’s account for a subscription purchase or renewal after a lapse, in the UNIX epoch time format, in milliseconds. Use this time format for processing dates.
        /// </summary>
        public string purchase_date_ms { get; set; }

        /// <summary>
        /// The time the App Store charged the user's account for a purchased or restored product, or the time the App Store charged the user’s account for a subscription purchase or renewal after a lapse, in the Pacific Time zone. 
        /// </summary>
        public string purchase_date_pst { get; set; }

        /// <summary>
        /// The number of consumable products purchased. This value corresponds to the quantity property of the SKPayment object stored in the transaction's payment property. The value is usually “1” unless modified with a mutable payment. The maximum value is 10.
        /// </summary>
        public string quantity { get; set; }

        /// <summary>
        /// A unique identifier for a transaction such as a purchase, restore, or renewal.
        /// </summary>
        public string transaction_id { get; set; }

        /// <summary>
        /// A unique identifier for purchase events across devices, including subscription-renewal events. This value is the primary key for identifying subscription purchases.
        /// </summary>
        public string web_order_line_item_id { get; set; }
    }
}
