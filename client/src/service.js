import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;
axios.defaults.baseURL = apiUrl;
axios.interceptors.response.use(
  response => response,  
  error => {
    console.error('API Request Error:', error); 
    return Promise.reject(error);  
  }
);

export default {

  getTasks: async () => {
    try {
      const result = await axios.get('/tasks');
      return result.data;
    } catch (error) {
      console.error("Error fetching tasks:", error);
      throw error;  
    }
  },


  addTask: async (name) => {
    try {
      const newTask = { Name: name }; 
      const result = await axios.post('/addItem', newTask);
      return result.data;
    } catch (error) {
      console.error("Error adding task:", error);
      throw error;
    }
  },

  
  setCompleted: async (id, isComplete) => {
    try {
      const result = await axios.put(`/tasks/updateItem/${id}`, { isComplete });
      return result.data;
    } catch (error) {
      console.error("Error updating task:", error);
      throw error;
    }
  },

  // פונקציה למחיקת משימה
  deleteTask: async (id) => {
    try {
      const result = await axios.delete(`/deleteItem/${id}`);
      return result.data;
    } catch (error) {
      console.error("Error deleting task:", error);
      throw error;
    }
  }
};
// export default {
//   getTasks: async () => {
//     const result = await axios.get(`${apiUrl}/tasks`)    
//     return result.data;
//   },

//   addTask: async(name)=>{
//     console.log('addTask', name)
//     //TODO
//     return {};
//   },

//   setCompleted: async(id, isComplete)=>{
//     console.log('setCompleted', {id, isComplete})
//     //TODO
//     return {};
//   },

//   deleteTask:async()=>{
//     console.log('deleteTask')
//   }
// };
