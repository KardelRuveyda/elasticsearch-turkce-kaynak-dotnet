<script>
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import Slider from 'primevue/slider'

export default {
  components: {
    DataTable,
    Column,
    InputText,
    Slider
  },
  data() {
    return {
      ecommerces: [],
      totalCount: 0,
      filters: {
        cityName: '',
        customerFullName: '',
        categoryName: '',
        manufacturer: '',
        taxfulTotalPriceGte: 0,
        taxfulTotalPriceLte: 50,
        pageSize: 10,
        page: 1
      },
      sliderValue: [0, 500]
    }
  },
  mounted() {
    this.fetchData()
  },
  watch: {
    sliderValue: function (oldValue, newValue) {
      console.log(oldValue)
      this.filters.taxfulTotalPriceGte = newValue[0]
      this.filters.taxfulTotalPriceLte = newValue[1]
    }
  },
  methods: {
    async fetchData() {
      const request = await fetch('https://localhost:11000/GetAllECommerceCustomQuery', {
        method: 'POST',
        body: JSON.stringify(this.filters),
        headers: {
          accept: 'application/json',
          'Content-Type': 'application/json'
        }
      })
      const data = await request.json()

      this.ecommerces = data.response
      this.totalCount = data.totalCount
    }
  }
}
</script>

<template>
  <div class="card">
    <InputText
      type="text"
      placeholder="Customer Name"
      v-model="filters.customerFullName"
      @keypress.enter="fetchData"
    />
    <InputText
      type="text"
      placeholder="City"
      v-model="filters.cityName"
      @keypress.enter="fetchData"
    />
    <Slider v-model="sliderValue" min="0" max="500" range class="w-14rem" @slideend="fetchData" />
    <InputText
      type="text"
      placeholder="Category"
      v-model="filters.categoryName"
      @keypress.enter="fetchData"
    />
  </div>
  <div class="card">
    <DataTable
      :value="ecommerces"
      paginator
      lazy
      :rows="filters.pageSize"
      :rowsPerPageOptions="[1, 5, 10, 20, 50]"
      tableStyle="min-width: 50rem"
      :totalRecords="totalCount"
      v-on:page="
        (e) => {
          this.filters.page = e.page + 1
          this.filters.pageSize = e.rows
          this.fetchData()
        }
      "
    >
      <Column field="customer_full_name" header="Customer Name" style="width: 25%"></Column>
      <Column field="geoip.city_name" header="City" style="width: 25%"></Column>
      <Column field="taxful_total_price" header="Taxful Total Price" style="width: 25%"></Column>
      <Column field="category" header="Category" style="width: 25%"></Column>
    </DataTable>
  </div>
</template>
