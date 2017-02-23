using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Crm.Orders.Model
{
    /// <summary>
    /// Order
    /// </summary>
    [DataContract]
    public partial class Order :  IEquatable<Order>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Order" /> class.
        /// </summary>
        /// <param name="OrderNumber">Unique identifier representing a specific order.</param>
        /// <param name="AccountNumber">Customer Number.</param>
        /// <param name="OrderDate">Date of the Estimate Completion Time.</param>
        /// <param name="ActualShipmentDate">Date of the shipment.</param>
        /// <param name="Freight">Cost of the Freight.</param>
        /// <param name="Tax">Cost of tax.</param>
        /// <param name="Total">Cost total.</param>
        /// <param name="Subtotal">Cost of all the items.</param>
        public Order(string OrderNumber = default(string), 
            string AccountNumber = default(string), 
            DateTime? OrderDate = default(DateTime?), 
            DateTime? ActualShipmentDate = default(DateTime?),
            DateTime? EstimatedShipmentDate = default(DateTime?),
            float? Freight = default(float?), 
            float? Tax = default(float?), 
            float? Total = default(float?), 
            float? Subtotal = default(float?),
            string Status = default(string))
        {
            this.OrderNumber = OrderNumber;
            this.AccountNumber = AccountNumber;
            this.OrderDate = OrderDate;
            this.ActualShipmentDate = ActualShipmentDate;
            this.EstimatedShipmentDate = EstimatedShipmentDate;
            this.Freight = Freight;
            this.Tax = Tax;
            this.Total = Total;
            this.Subtotal = Subtotal;
            this.Status = Status;
        }
        
        /// <summary>
        /// Unique identifier representing a specific order
        /// </summary>
        /// <value>Unique identifier representing a specific order</value>
        [DataMember(Name="order_number", EmitDefaultValue=false)]
        public string OrderNumber { get; set; }
        /// <summary>
        /// Customer Number
        /// </summary>
        /// <value>Customer Number</value>
        [DataMember(Name="account_number", EmitDefaultValue=false)]
        public string AccountNumber { get; set; }
        /// <summary>
        /// Date of the Estimate Completion Time
        /// </summary>
        /// <value>Date of the Estimate Completion Time</value>
        [DataMember(Name="order_date", EmitDefaultValue=false)]
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Date of the shipment
        /// </summary>
        /// <value>Date of the shipment</value>
        [DataMember(Name="actual_shipment_date", EmitDefaultValue=false)]
        public DateTime? ActualShipmentDate { get; set; }

        /// <summary>
        /// Date of the shipment
        /// </summary>
        /// <value>Date of the shipment</value>
        [DataMember(Name = "estimated_shipment_date", EmitDefaultValue = false)]
        public DateTime? EstimatedShipmentDate { get; set; }

        /// <summary>
        /// Cost of the Freight
        /// </summary>
        /// <value>Cost of the Freight</value>
        [DataMember(Name="freight", EmitDefaultValue=false)]
        public float? Freight { get; set; }
        /// <summary>
        /// Cost of tax
        /// </summary>
        /// <value>Cost of tax</value>
        [DataMember(Name="tax", EmitDefaultValue=false)]
        public float? Tax { get; set; }
        /// <summary>
        /// Cost total
        /// </summary>
        /// <value>Cost total</value>
        [DataMember(Name="total", EmitDefaultValue=false)]
        public float? Total { get; set; }

        /// <summary>
        /// Cost of all the items
        /// </summary>
        /// <value>Cost of all the items</value>
        [DataMember(Name="subtotal", EmitDefaultValue=false)]
        public float? Subtotal { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("class Order {\n");
            sb.Append("  OrderNumber: ").Append(OrderNumber).Append("\n");
            sb.Append("  CustomerNumber: ").Append(AccountNumber).Append("\n");
            sb.Append("  OrderDate: ").Append(OrderDate).Append("\n");
            sb.Append("  ShipmentDate: ").Append(ActualShipmentDate).Append("\n");
            sb.Append("  Freight: ").Append(Freight).Append("\n");
            sb.Append("  Tax: ").Append(Tax).Append("\n");
            sb.Append("  Total: ").Append(Total).Append("\n");
            sb.Append("  Subtotal: ").Append(Subtotal).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            return this.Equals(obj as Order);
        }

        /// <summary>
        /// Returns true if Order instances are equal
        /// </summary>
        /// <param name="other">Instance of Order to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Order other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.OrderNumber == other.OrderNumber ||
                    this.OrderNumber != null &&
                    this.OrderNumber.Equals(other.OrderNumber)
                ) && 
                (
                    this.AccountNumber == other.AccountNumber ||
                    this.AccountNumber != null &&
                    this.AccountNumber.Equals(other.AccountNumber)
                ) && 
                (
                    this.OrderDate == other.OrderDate ||
                    this.OrderDate != null &&
                    this.OrderDate.Equals(other.OrderDate)
                ) && 
                (
                    this.ActualShipmentDate == other.ActualShipmentDate ||
                    this.ActualShipmentDate != null &&
                    this.ActualShipmentDate.Equals(other.ActualShipmentDate)
                ) && 
                (
                    this.Freight == other.Freight ||
                    this.Freight != null &&
                    this.Freight.Equals(other.Freight)
                ) && 
                (
                    this.Tax == other.Tax ||
                    this.Tax != null &&
                    this.Tax.Equals(other.Tax)
                ) && 
                (
                    this.Total == other.Total ||
                    this.Total != null &&
                    this.Total.Equals(other.Total)
                ) && 
                (
                    this.Subtotal == other.Subtotal ||
                    this.Subtotal != null &&
                    this.Subtotal.Equals(other.Subtotal)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                if (this.OrderNumber != null)
                    hash = hash * 59 + this.OrderNumber.GetHashCode();
                if (this.AccountNumber != null)
                    hash = hash * 59 + this.AccountNumber.GetHashCode();
                if (this.OrderDate != null)
                    hash = hash * 59 + this.OrderDate.GetHashCode();
                if (this.ActualShipmentDate != null)
                    hash = hash * 59 + this.ActualShipmentDate.GetHashCode();
                if (this.Freight != null)
                    hash = hash * 59 + this.Freight.GetHashCode();
                if (this.Tax != null)
                    hash = hash * 59 + this.Tax.GetHashCode();
                if (this.Total != null)
                    hash = hash * 59 + this.Total.GetHashCode();
                if (this.Subtotal != null)
                    hash = hash * 59 + this.Subtotal.GetHashCode();
                return hash;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            yield break;
        }
    }

}
